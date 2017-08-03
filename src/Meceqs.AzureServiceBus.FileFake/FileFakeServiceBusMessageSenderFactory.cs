using System.IO;
using Meceqs.AzureServiceBus.Internal;
using Microsoft.Extensions.Logging;

namespace Meceqs.AzureServiceBus.FileFake
{
    public class FileFakeServiceBusMessageSenderFactory : IServiceBusMessageSenderFactory
    {
        private readonly string _directory;
        private readonly ILoggerFactory _loggerFactory;

        public FileFakeServiceBusMessageSenderFactory(string directory, ILoggerFactory loggerFactory)
        {
            Guard.NotNullOrWhiteSpace(directory, nameof(directory));
            Guard.NotNull(loggerFactory, nameof(loggerFactory));

            _directory = directory;
            _loggerFactory = loggerFactory;

            EnsureDirectoryExists();
        }

        public IServiceBusMessageSender CreateMessageSender(string connectionString, string entityPath)
        {
            Guard.NotNullOrWhiteSpace(entityPath, nameof(entityPath));

            string entityPathDirectory = Path.Combine(_directory, entityPath);

            return new FileFakeServiceBusMessageSender(entityPathDirectory, _loggerFactory);
        }

        private void EnsureDirectoryExists()
        {
            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }
        }
    }
}