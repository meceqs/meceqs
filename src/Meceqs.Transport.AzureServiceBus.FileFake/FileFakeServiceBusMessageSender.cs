using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Meceqs.Transport.AzureServiceBus.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace Meceqs.Transport.AzureServiceBus.FileFake
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
            
            // The body can only be read once.
            string serializedEnvelope;
            using (StreamReader reader = new StreamReader(message.GetBody<Stream>(), Encoding.UTF8))
            {
                serializedEnvelope = reader.ReadToEnd();
            }

            InvokeWithRetry(3, () =>
            {
                var sb = new StringBuilder();
                var sw = new StringWriter(sb);

                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    writer.WriteStartObject();

                    foreach (var kvp in message.Properties)
                    {
                        writer.WritePropertyName(kvp.Key);
                        writer.WriteValue(kvp.Value);
                    }

                    writer.WritePropertyName(nameof(message.MessageId));
                    writer.WriteValue(message.MessageId);

                    writer.WritePropertyName(nameof(message.CorrelationId));
                    writer.WriteValue(message.CorrelationId);
                    
                    writer.WritePropertyName("Body");
                    writer.WriteValue(serializedEnvelope);

                    writer.WriteEndObject();
                }

                File.WriteAllText(fileName, sb.ToString());
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