using Meceqs.AzureEventHubs.Receiving;
using Meceqs.Transport;

namespace Meceqs.AzureEventHubs.Configuration
{
    public class EventHubReceiverBuilder : TransportReceiverBuilder<IEventHubReceiverBuilder, EventHubReceiverOptions>,
        IEventHubReceiverBuilder
    {
        public override IEventHubReceiverBuilder Instance => this;
    }
}