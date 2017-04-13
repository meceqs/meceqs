using System;
using System.IO;
using System.Threading.Tasks;
using Meceqs.AzureEventHubs.Internal;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Logging;

namespace Meceqs.AzureEventHubs.FileFake
{
    public class FileFakeEventHubClient : IEventHubClient
    {
        private readonly string _fileName;
        private readonly ILogger _logger;

        public FileFakeEventHubClient(string fileName, ILoggerFactory loggerFactory)
        {
            Check.NotNullOrWhiteSpace(fileName, nameof(fileName));
            Check.NotNull(loggerFactory, nameof(loggerFactory));

            _fileName = fileName;
            _logger = loggerFactory.CreateLogger<FileFakeEventHubClient>();

            EnsureDirectoryExists();
        }

        public Task SendAsync(EventData data, string partitionKey)
        {
            // TODO @cweiss partition algorithm?

            string serializedEventData = FileFakeEventDataSerializer.Serialize(data);

            InvokeWithRetry(3, () =>
            {
                File.AppendAllLines(_fileName, new[] { serializedEventData });
            });

            return Task.CompletedTask;
        }

        public void Close()
        {
        }

        private void InvokeWithRetry(int attempts, Action action)
        {
            int attempt = 0;
            do
            {
                try
                {
                    attempt++;
                    action();
                    break;
                }
                catch (Exception ex) when (attempt <= attempts)
                {
                    _logger.LogWarning(0, ex, "Retrying");

                    // if someone deleted the root folder...
                    if (ex is DirectoryNotFoundException)
                    {
                        EnsureDirectoryExists();
                    }
                }

            } while (true);
        }

        private void EnsureDirectoryExists()
        {
            var directory = Path.GetDirectoryName(_fileName);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}