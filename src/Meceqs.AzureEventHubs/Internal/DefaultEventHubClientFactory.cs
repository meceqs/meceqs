using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureEventHubs.Internal
{
    public class DefaultEventHubClientFactory : IEventHubClientFactory
    {
        public EventHubClient CreateEventHubClient(EventHubConnection connection)
        {
            Check.NotNull(connection, nameof(connection));

            return EventHubClient.CreateFromConnectionString(connection.ConnectionString, connection.EventHubName);
        }
    }
}