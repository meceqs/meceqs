using System.IO;
using Meceqs.Transport.AzureServiceBus.Internal;
using Microsoft.Extensions.Logging;

namespace Meceqs.Transport.AzureServiceBus.FileMock
{
    public class FileMockServiceBusMessageSenderFactory : IServiceBusMessageSenderFactory
    {


        private readonly string _directory;
        private readonly ILoggerFactory _loggerFactory;

        public FileMockServiceBusMessageSenderFactory(string directory, ILoggerFactory loggerFactory)
        {
            Check.NotNullOrWhiteSpace(directory, nameof(directory));
            Check.NotNull(loggerFactory, nameof(loggerFactory));

            _directory = directory;
            _loggerFactory = loggerFactory;

            EnsureDirectoryExists();
        }

        public IServiceBusMessageSender CreateMessageSender(string connectionString, string entityPath)
        {
            Check.NotNullOrWhiteSpace(entityPath, nameof(entityPath));

            string entityPathDirectory = Path.Combine(_directory, entityPath);

            return new FileMockServiceBusMessageSender(entityPathDirectory, _loggerFactory);
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