using Microsoft.Azure.EventHubs;

namespace Meceqs.AzureEventHubs.Internal
{
    public class DefaultEventHubClientFactory : IEventHubClientFactory
    {
        public IEventHubClient CreateEventHubClient(EventHubConnection connection)
        {
            Check.NotNull(connection, nameof(connection));

            var connectionBuilder = new EventHubsConnectionStringBuilder(connection.ConnectionString);

            connectionBuilder.EntityPath = connection.EventHubName;

            var client = EventHubClient.CreateFromConnectionString(connectionBuilder.ToString());

            return new EventHubClientWrapper(client);
        }
    }
}