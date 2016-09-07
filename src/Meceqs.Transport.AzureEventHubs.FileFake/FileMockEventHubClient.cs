using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Meceqs.Transport.AzureEventHubs.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.Transport.AzureEventHubs.FileMock
{
    public class FileMockEventHubClient : IEventHubClient
    {
        private readonly string _fileName;
        private readonly ILogger _logger;

        public FileMockEventHubClient(string fileName, ILoggerFactory loggerFactory)
        {
            Check.NotNullOrWhiteSpace(fileName, nameof(fileName));
            Check.NotNull(loggerFactory, nameof(loggerFactory));

            _fileName = fileName;
            _logger = loggerFactory.CreateLogger<FileMockEventHubClient>();

            EnsureFileExists();
        }

        public Task SendAsync(EventData data)
        {
            InvokeWithRetry(3, () =>
            {
                using (StreamReader reader = new StreamReader(data.GetBodyStream(), Encoding.UTF8))
                {
                    string json = reader.ReadToEnd();
                    File.AppendAllLines(_fileName, new[] { json });
                }
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