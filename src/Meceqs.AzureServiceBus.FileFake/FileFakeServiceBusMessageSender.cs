using System;
using System.IO;
using System.Threading.Tasks;
using Meceqs.AzureServiceBus.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureServiceBus.FileFake
{
    public class FileFakeServiceBusMessageSender : IServiceBusMessageSender
    {
        private readonly string _entityPathDirectory;
        private readonly ILogger _logger;

        public FileFakeServiceBusMessageSender(string entityPathDirectory, ILoggerFactory loggerFactory)
        {
            Check.NotNullOrWhiteSpace(entityPathDirectory, nameof(entityPathDirectory));
            Check.NotNull(loggerFactory, nameof(loggerFactory));

            _entityPathDirectory = entityPathDirectory;
            _logger = loggerFactory.CreateLogger<FileFakeServiceBusMessageSender>();

            EnsureDirectoryExists();
        }

        public Task SendAsync(BrokeredMessage message)
        {
            string fileName = Path.Combine(_entityPathDirectory, $"{message.MessageId}.json");
            
            string serializedMessage = FileFakeBrokeredMessageSerializer.Serialize(message);

            InvokeWithRetry(3, () =>
            {
                File.WriteAllText(fileName, serializedMessage);
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
                        EnsureDirectoryExists();
                    }
                }

            } while (true);
        }

        private void EnsureDirectoryExists()
        {
            if (!Directory.Exists(_entityPathDirectory))
            {
                Directory.CreateDirectory(_entityPathDirectory);
            }
        }
    }
}