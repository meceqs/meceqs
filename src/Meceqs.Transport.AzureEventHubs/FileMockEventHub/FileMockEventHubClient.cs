using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Meceqs.Transport.AzureEventHubs.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.Transport.AzureEventHubs.FileMockEventHub
{
    public class FileMockEventHubClient : IEventHubClient
    {
        private readonly FileMockOptions _options;
        private readonly ILogger _logger;

        public FileMockEventHubClient(IOptions<FileMockOptions> options, ILoggerFactory loggerFactory)
        {
            Check.NotNull(options?.Value, nameof(options));
            Check.NotNull(loggerFactory, nameof(loggerFactory));

            _options = options.Value;
            _logger = loggerFactory.CreateLogger<FileMockEventHubClient>();

            EnsureFileExists();
        }

        public Task SendAsync(EventData data)
        {
            using (StreamReader reader = new StreamReader(data.GetBodyStream(), Encoding.UTF8))
            {
                string json = reader.ReadToEnd();
                File.AppendAllLines(_options.Filename, new [] { json });
            }

            return Task.CompletedTask;
        }

        public void Close()
        {
        }

        private void InvokeWithRetry(Action action, int attempts)
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
                }

            } while (true);
        }

        private void EnsureFileExists()
        {
            var directory = Path.GetDirectoryName(_options.Filename);

            if (_options.CreateDirectory && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(_options.Filename))
            {
                File.Create(_options.Filename);
            }
        }
    }
}