using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.AzureEventHubs.Receiving;
using Microsoft.Extensions.Logging;

namespace Meceqs.AzureEventHubs.FileFake
{
    public class FileFakeEventHubProcessor
    {
        private static readonly Random _random = new Random();

        private readonly FileFakeEventHubProcessorOptions _options;
        private readonly string _fileName;


        private readonly IEventHubReceiver _eventHubReceiver;
        private readonly ILogger _logger;

        private Timer _processingTimer;

        private long _fileCreationTicks = 0;
        private int _sequenceNumber = 0;

        public FileFakeEventHubProcessor(FileFakeEventHubProcessorOptions options, IEventHubReceiver eventHubReceiver, ILoggerFactory loggerFactory)
        {
            Guard.NotNull(options, nameof(options));

            _options = options;
            _eventHubReceiver = eventHubReceiver;
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

            List<string> newEvents = null;
            int count = 0;
            while (newEvents == null)
            {
                try
                {
                    count++;
                    newEvents = File.ReadLines(_fileName).Skip(_sequenceNumber).ToList();
                }
                catch (Exception) when (count <= 3)
                {
                    await Task.Delay(35);
                }
            }

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
            Guard.NotNull(serializedEvent, nameof(serializedEvent));

            var eventData = FileFakeEventDataSerializer.Deserialize(serializedEvent, sequenceNumber);

            // The receiver will create a new scope, so we don't have to do it here.
            var cancellationToken = CancellationToken.None; // TODO @cweiss CancellationToken???
            await _eventHubReceiver.ReceiveAsync(eventData, cancellationToken);
        }
    }
}