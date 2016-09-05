namespace Meceqs.Transport.AzureEventHubs.Internal
{
    /// <summary>
    /// This interface allows to mock an EventHubClient.
    /// </summary>
    public interface IEventHubClientFactory
    {
        IEventHubClient CreateEventHubClient(EventHubConnection connection);
    }
}