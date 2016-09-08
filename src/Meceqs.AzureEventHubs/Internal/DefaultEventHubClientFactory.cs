using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureEventHubs.Internal
{
    public class DefaultEventHubClientFactory : IEventHubClientFactory
    {
        public IEventHubClient CreateEventHubClient(EventHubConnection connection)
        {
            Check.NotNull(connection, nameof(connection));

            var client = EventHubClient.CreateFromConnectionString(connection.ConnectionString, connection.EventHubName);
            return new EventHubClientWrapper(client);
        }
    }
}