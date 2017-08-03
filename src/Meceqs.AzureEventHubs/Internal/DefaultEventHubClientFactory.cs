using Microsoft.Azure.EventHubs;

namespace Meceqs.AzureEventHubs.Internal
{
    public class DefaultEventHubClientFactory : IEventHubClientFactory
    {
        public IEventHubClient CreateEventHubClient(string connectionString)
        {
            Guard.NotNullOrWhiteSpace(connectionString, nameof(connectionString));

            var connectionBuilder = new EventHubsConnectionStringBuilder(connectionString);
            var client = EventHubClient.CreateFromConnectionString(connectionBuilder.ToString());

            return new EventHubClientWrapper(client);
        }
    }
}