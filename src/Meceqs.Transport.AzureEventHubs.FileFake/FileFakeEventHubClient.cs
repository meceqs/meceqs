using System;
using System.IO;
using System.Threading.Tasks;
using Meceqs.Transport.AzureEventHubs.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.Transport.AzureEventHubs.FileFake
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

            EnsureFileExists();
        }

        public Task SendAsync(EventData data)
        {
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
                catch (Exception ex)
                {
                    if (attempt >= attempts)
                    {
                        throw;
                    }

                    _logger.LogWarning(0, ex, "Retrying");

                    // if someone deleted the root folder...
                    if (ex is DirectoryNotFoundException)
                    {
                        EnsureFileExists();
                    }
                }

            } while (true);
        }

        private void EnsureFileExists()
        {
            var directory = Path.GetDirectoryName(_fileName);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(_fileName))
            {
                File.Create(_fileName);
            }
        }
    }
}