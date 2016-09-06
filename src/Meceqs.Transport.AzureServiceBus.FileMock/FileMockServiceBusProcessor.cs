using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.Transport.AzureServiceBus.Consuming;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json.Linq;

namespace Meceqs.Transport.AzureServiceBus.FileMock
{
    public class FileMockServiceBusProcessor
    {
        private static readonly Random _random = new Random();

        private readonly FileMockServiceBusProcessorOptions _options;
        private readonly string _directory;
        private readonly string _archiveDirectory;


        private readonly IServiceProvider _applicationServices;
        private readonly ILogger _logger;

        private Timer _processingTimer;

        public FileMockServiceBusProcessor(
            FileMockServiceBusProcessorOptions options,
            IServiceProvider applicationServices,
            ILoggerFactory loggerFactory)
        {
            Check.NotNull(options, nameof(options));

            _options = options;
            _applicationServices = applicationServices;
            _logger = loggerFactory.CreateLogger<FileMockServiceBusProcessor>();

            if (string.IsNullOrWhiteSpace(_options.Directory))
                throw new ArgumentNullException(nameof(_options.Directory));
            
            if (string.IsNullOrEmpty(_options.EntityPath))
                throw new ArgumentNullException(nameof(_options.EntityPath));

            _directory = Path.Combine(_options.Directory, _options.EntityPath);
            _archiveDirectory = Path.Combine(_directory, _options.ArchiveFolderName);
        }

        public void Start()
        {
            EnsureInitialState();

            _logger.LogInformation("Starting timers");

            _processingTimer = new Timer(_ =>
            {
                InvokeTimerMethod("ProcessMessages", ReadMessages, _processingTimer, 1 * 3000);
            }, null, _random.Next(1000) /* first start */, Timeout.Infinite);
        }

        private void EnsureInitialState()
        {
            if (!Directory.Exists(_options.Directory))
            {
                Directory.CreateDirectory(_options.Directory);
            }
            else if (_options.ClearOnStart)
            {
                var di = new DirectoryInfo(_options.Directory);

                foreach (var file in di.GetFiles())
                {
                    file.Delete();
                }

                foreach (var dir in di.GetDirectories())
                {
                    dir.Delete(recursive: true);
                }
            }

            if (!Directory.Exists(_archiveDirectory))
            {
                Directory.CreateDirectory(_archiveDirectory);
            }
        }

        private async Task ReadMessages()
        {
            FileInfo[] files = null;

            if (Directory.Exists(_directory))
            {
                var di = new DirectoryInfo(_directory);
                files = di.GetFiles();
            }

            _logger.LogInformation("Processing {Count} messages from {Directory}", files?.Length, _directory);

            if (files?.Length > 0)
            {
                foreach (var file in files)
                {
                    string archiveFilePath = Path.Combine(_archiveDirectory, file.Name);

                    string fileContent = File.ReadAllText(file.FullName);
                    await ProcessFile(fileContent);

                    // "commit" by moving it to a subdirectory.
                    File.Move(file.FullName, archiveFilePath);
                }

                _logger.LogInformation("Processing finished");
            }
        }

        private void InvokeTimerMethod(string name, Func<Task> action, Timer timer, int interval)
        {
            try
            {
                action().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Exception on {Method}", name);
            }
            finally
            {
                // restart timer
                timer.Change(interval, Timeout.Infinite);
            }
        }

        private async Task ProcessFile(string fileContent)
        {
            var brokeredMessage = DeserializeBrokeredMessage(fileContent);

            var serviceScopeFactory = _applicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var requestServices = scope.ServiceProvider;
                var serviceBusConsumer = requestServices.GetRequiredService<IServiceBusConsumer>();

                await serviceBusConsumer.ConsumeAsync(brokeredMessage, CancellationToken.None);
            }
        }

        private BrokeredMessage DeserializeBrokeredMessage(string serializedMessage)
        {
            JObject jsonMessage = JObject.Parse(serializedMessage);

            string serializedEnvelope = jsonMessage.GetValue("Body").ToString();
            MemoryStream payloadStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedEnvelope));
            BrokeredMessage brokeredMessage = new BrokeredMessage(payloadStream, ownsStream: true);

            foreach (var header in TransportHeaderNames.AsList())
            {
                string value = jsonMessage.GetValue(header)?.ToString();
                brokeredMessage.Properties[header] = value;
            }

            brokeredMessage.ContentType = brokeredMessage.Properties[TransportHeaderNames.ContentType].ToString();
            brokeredMessage.MessageId = brokeredMessage.Properties[TransportHeaderNames.MessageId].ToString();

            brokeredMessage.CorrelationId = jsonMessage.GetValue(nameof(brokeredMessage.CorrelationId)).ToString();

            // Receiver properties are internal - that's why we need reflection :(
            // (if these properties are not set, accessing them will throw an exception.)

            SetPropertyValue(brokeredMessage, nameof(brokeredMessage.DeliveryCount), 1);
            SetPropertyValue(brokeredMessage, nameof(brokeredMessage.SequenceNumber), 1);
            SetPropertyValue(brokeredMessage, nameof(brokeredMessage.EnqueuedSequenceNumber), 1);
            //SetReceiveContext(brokeredMessage);

            return brokeredMessage;
        }

        private void SetPropertyValue(BrokeredMessage message, string propertyName, object value)
        {
            typeof(BrokeredMessage).GetProperty(propertyName).SetValue(message, value);
        }

        // private void SetReceiveContext(BrokeredMessage message)
        // {
        //     // now it gets really ugly. Calls to Complete() etc require a ReceiveContext
        //     // which is internal.

        //     var messageReceiver = Substitute.For<MessageReceiver>();
        //     var assembly = Assembly.Load("Microsoft.ServiceBus");
        //     var receiveContextType = assembly.GetType("Microsoft.ServiceBus.Messaging.ReceiveContext", throwOnError: true);

        //     var ctor = receiveContextType.GetConstructor(BindingFlags.Public, null, new Type[] { typeof(MessageReceiver), typeof(Guid) }, null);
        //     object receiveContext = ctor.Invoke(new object[] { messageReceiver, Guid.NewGuid() });

        //     typeof(BrokeredMessage).GetProperty("ReceiveContext").SetValue(message, receiveContext);
        // }
    }
}