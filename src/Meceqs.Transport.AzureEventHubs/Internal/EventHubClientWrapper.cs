using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.Transport.AzureEventHubs.Internal
{
    public class EventHubClientWrapper : IEventHubClient
    {
        private readonly EventHubClient _client;

        public EventHubClientWrapper(EventHubClient client)
        {
            Check.NotNull(client, nameof(client));

            _client = client;
        }

        public Task SendAsync(EventData data)
        {
            return _client.SendAsync(data);
        }

        public void Close()
        {
            _client.Close();
        }
    }
}