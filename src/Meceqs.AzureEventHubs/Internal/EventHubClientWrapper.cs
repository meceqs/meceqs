using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;

namespace Meceqs.AzureEventHubs.Internal
{
    public class EventHubClientWrapper : IEventHubClient
    {
        private readonly EventHubClient _client;

        public EventHubClientWrapper(EventHubClient client)
        {
            Guard.NotNull(client, nameof(client));

            _client = client;
        }

        public Task SendAsync(EventData data, string partitionKey)
        {
            return _client.SendAsync(data, partitionKey);
        }

        public void Close()
        {
            _client.Close();
        }
    }
}
