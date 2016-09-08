using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.AzureEventHubs.Consuming;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Meceqs.AzureEventHubs.FileFake
{
    public class FileFakeEventHubProcessor
    {
        private static readonly Random _random = new Random();

        private readonly FileFakeEventHubProcessorOptions _options;
        private readonly string _fileName;


        private readonly IServiceProvider _applicationServices;
        private readonly ILogger _logger;

        private Timer _processingTimer;

        private long _fileCreationTicks = 0;
        private int _sequenceNumber = 0;

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
            if (!File.Exists(_fileName))
            {
                _sequenceNumber = 0;
                _logger.LogInformation("No events. File does not exist: {File}", _fileName);
                return;
            }

            var fileInfo = new FileInfo(_fileName);

            // We cache the creation date to make sure we can re-start if someone deletes the file.
            
            var creationTicks = fileInfo.CreationTimeUtc.Ticks;
            if (_fileCreationTicks != creationTicks)
            {
                if (_fileCreationTicks > 0)
                {
                    // Someone deleted the file -> restart from the beginning!
                    _logger.LogInformation("File creation date doesn't match. Resetting sequence number to 0");
                }
                
                _sequenceNumber = 0;
                _fileCreationTicks = creationTicks;
            }

            // Start processing.

            var newEvents = File.ReadLines(_fileName).Skip(_sequenceNumber).ToList();

            _logger.LogInformation("Processing {Count} events, SequenceNumber: {SequenceNumber}", newEvents.Count,  _sequenceNumber);

            if (newEvents.Count > 0)
            {
                // process events (if any)
                foreach (var evt in newEvents)
                {
                    await ProcessEvent(evt, _sequenceNumber);

                    // "commit" by moving cursor forward.
                    _sequenceNumber++;
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

        private async Task ProcessEvent(string serializedEvent, int sequenceNumber)
        {
            Check.NotNull(serializedEvent, nameof(serializedEvent));

            var eventData = FileFakeEventDataSerializer.Deserialize(serializedEvent, sequenceNumber);

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