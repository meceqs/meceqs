using System.Collections.Concurrent;
using System.IO;
using Meceqs.AzureEventHubs.Internal;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Logging;

namespace Meceqs.AzureEventHubs.FileFake
{
    public class FileFakeEventHubClientFactory : IEventHubClientFactory
    {
        private readonly string _directory;
        private readonly ILoggerFactory _loggerFactory;

        private readonly ConcurrentDictionary<string, IEventHubClient> _clients = new ConcurrentDictionary<string, IEventHubClient>();

        public FileFakeEventHubClientFactory(string directory, ILoggerFactory loggerFactory)
        {
            Guard.NotNullOrWhiteSpace(directory, nameof(directory));
            Guard.NotNull(loggerFactory, nameof(loggerFactory));

            _directory = directory;
            _loggerFactory = loggerFactory;

            EnsureDirectoryExists();
        }

        public IEventHubClient CreateEventHubClient(string connectionString)
        {
            Guard.NotNullOrWhiteSpace(connectionString, nameof(connectionString));

            return _clients.GetOrAdd(connectionString, key =>
            {
                string entityPath = new EventHubsConnectionStringBuilder(connectionString).EntityPath;

                string fileName = Path.Combine(_directory, $"{entityPath}.txt");

                return new FileFakeEventHubClient(fileName, _loggerFactory);
            });
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