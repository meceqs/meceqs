using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureEventHubs.Internal
{
    /// <summary>
    /// This interface allows to mock an EventHubClient.
    /// </summary>
    public interface IEventHubClientFactory
    {
        EventHubClient CreateEventHubClient(EventHubConnection connection);
    }
}