using Meceqs.Transport;

namespace Meceqs.AzureEventHubs.Sending
{
    public class EventHubSenderOptions : TransportSenderOptions
    {
        public string EventHubConnectionString { get; set; }
    }
}