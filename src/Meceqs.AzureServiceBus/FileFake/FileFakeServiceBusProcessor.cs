using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.AzureServiceBus.Receiving;
using Microsoft.Extensions.Logging;

namespace Meceqs.AzureServiceBus.FileFake
{
    public class FileFakeServiceBusProcessor
    {
        private static readonly Random _random = new Random();

        private bool _clearOnStart;
        private readonly string _directory;
        private readonly string _archiveDirectory;

        private readonly IServiceBusReceiver _serviceBusReceiver;
        private readonly ILogger _logger;

        private Timer _processingTimer;

        public FileFakeServiceBusProcessor(
            FileFakeServiceBusProcessorOptions options,
            IServiceBusReceiver serviceBusReceiver,
            ILoggerFactory loggerFactory)
        {
            Guard.NotNull(options, nameof(options));

            _serviceBusReceiver = serviceBusReceiver;
            _logger = loggerFactory.CreateLogger<FileFakeServiceBusProcessor>();

            if (string.IsNullOrWhiteSpace(options.Directory))
                throw new ArgumentNullException(nameof(options.Directory));

            if (string.IsNullOrEmpty(options.EntityPath))
                throw new ArgumentNullException(nameof(options.EntityPath));

            _clearOnStart = options.ClearOnStart;
            _directory = Path.Combine(options.Directory, options.EntityPath);
            _archiveDirectory = Path.Combine(_directory, options.ArchiveFolderName);
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
            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }
            else if (_clearOnStart)
            {
                var di = new DirectoryInfo(_directory);

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
            var brokeredMessage = FileFakeServiceBusMessageSerializer.Deserialize(fileContent);

            // The receiver will create a new scope, so we don't have to do it here.
            var cancellationToken = CancellationToken.None; // TODO @cweiss CancellationToken???
            await _serviceBusReceiver.ReceiveAsync(brokeredMessage, cancellationToken);
        }
    }
}
