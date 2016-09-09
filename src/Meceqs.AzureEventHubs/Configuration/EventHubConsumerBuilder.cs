using Meceqs.AzureEventHubs.Consuming;
using Meceqs.Transport;

namespace Meceqs.AzureEventHubs.Configuration
{
    public class EventHubConsumerBuilder : TransportConsumerBuilder<IEventHubConsumerBuilder, EventHubConsumerOptions>,
        IEventHubConsumerBuilder
    {
        public override IEventHubConsumerBuilder Instance => this;
    }
}