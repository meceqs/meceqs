using System.Collections.Concurrent;
using System.IO;
using Meceqs.Transport.AzureEventHubs.Internal;
using Microsoft.Extensions.Logging;

namespace Meceqs.Transport.AzureEventHubs.FileFake
{
    public class FileFakeEventHubClientFactory : IEventHubClientFactory
    {
        private readonly string _directory;
        private readonly ILoggerFactory _loggerFactory;

        private readonly ConcurrentDictionary<EventHubConnection, IEventHubClient> _clients = new ConcurrentDictionary<EventHubConnection, IEventHubClient>();

        public FileFakeEventHubClientFactory(string directory, ILoggerFactory loggerFactory)
        {
            Check.NotNullOrWhiteSpace(directory, nameof(directory));
            Check.NotNull(loggerFactory, nameof(loggerFactory));

            _directory = directory;
            _loggerFactory = loggerFactory;

            EnsureDirectoryExists();
        }

        public IEventHubClient CreateEventHubClient(EventHubConnection connection)
        {
            Check.NotNull(connection, nameof(connection));

            return _clients.GetOrAdd(connection, key => 
            {
                string fileName = Path.Combine(_directory, $"{connection.EventHubName}.txt");

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