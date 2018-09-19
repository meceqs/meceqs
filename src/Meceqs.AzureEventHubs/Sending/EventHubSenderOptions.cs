using Meceqs.Transport;

namespace Meceqs.AzureEventHubs.Sending
{
    public class EventHubSenderOptions : SendTransportOptions
    {
        public string EventHubConnectionString { get; set; }
    }
}
