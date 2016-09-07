using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.Transport.AzureEventHubs.Consuming;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json.Linq;

namespace Meceqs.Transport.AzureEventHubs.FileFake
{
    public class FileFakeEventHubProcessor
    {
        private static readonly Random _random = new Random();

        private readonly FileFakeEventHubProcessorOptions _options;
        private readonly string _fileName;


        private readonly IServiceProvider _applicationServices;
        private readonly ILogger _logger;

        private Timer _processingTimer;

        private int _lineNumber = 0;

        public FileFakeEventHubProcessor(FileFakeEventHubProcessorOptions options, IServiceProvider applicationServices, ILoggerFactory loggerFactory)
        {
            Check.NotNull(options, nameof(options));

            _options = options;
            _applicationServices = applicationServices;
            _logger = loggerFactory.CreateLogger<FileFakeEventHubProcessor>();

            _fileName = Path.Combine(_options.Directory, $"{_options.EventHubName}.txt");
        }

        public void Start()
        {
            EnsureInitialState();

            _logger.LogInformation("Starting timers");

            _processingTimer = new Timer(_ =>
            {
                InvokeTimerMethod("ProcessEvents", ReadEvents, _processingTimer, 1 * 3000);
            }, null, _random.Next(1000) /* first start */, Timeout.Infinite);
        }

        private void EnsureInitialState()
        {
            if (!Directory.Exists(_options.Directory))
            {
                Directory.CreateDirectory(_options.Directory);
            }

            if (_options.ClearEventHubOnStart && File.Exists(_fileName))
            {
                File.Delete(_fileName);
            }
        }

        private async Task ReadEvents()
        {
            int count = 0;
            IEnumerable<string> events = null;

            if (File.Exists(_fileName))
            {
                // Read new events
                events = File.ReadLines(_fileName).Skip(_lineNumber);
                count = events.Count();
            }
            else
            {
                if (_lineNumber > 0)
                {
                    // Someone deleted the file -> restart from the beginning!
                    // TODO @cweiss this only works if the processor happens to access the file next.
                    _lineNumber = 0;
                    _logger.LogInformation("File deleted. Resetting offset to 0");
                }
            }

            _logger.LogInformation("Processing {Count} events, Offset: {Offset}", count, _lineNumber);

            if (events != null && count > 0)
            {
                // process events (if any)
                foreach (var evt in events)
                {
                    await ProcessEvent(evt);

                    // "commit" by moving cursor forward.
                    _lineNumber++;
                }

                _logger.LogInformation("Processing finished");
            }
        }

        private void InvokeTimerMethod(string name, Func<Task> action, Timer timer, int interval)
        {
            try
            {
                //_logger.LogInformation("Executing timer method {Method}", name);

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

        private async Task ProcessEvent(string serializedEvent)
        {
            Check.NotNull(serializedEvent, nameof(serializedEvent));

            var eventData = FileFakeEventDataSerializer.Deserialize(serializedEvent);

            var serviceScopeFactory = _applicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var requestServices = scope.ServiceProvider;
                var eventHubConsumer = requestServices.GetRequiredService<IEventHubConsumer>();

                await eventHubConsumer.ConsumeAsync(eventData, CancellationToken.None);
            }
        }
    }
}