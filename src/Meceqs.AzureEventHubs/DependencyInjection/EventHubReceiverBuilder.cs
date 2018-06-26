using Meceqs.AzureEventHubs.Receiving;
using Meceqs.Transport;

namespace Microsoft.Extensions.DependencyInjection
{
    public class EventHubReceiverBuilder : ReceiveTransportBuilder<EventHubReceiverBuilder, EventHubReceiverOptions>
    {
        protected override EventHubReceiverBuilder Instance => this;

        public EventHubReceiverBuilder(IMeceqsBuilder meceqsBuilder, string pipelineName)
            : base(meceqsBuilder, pipelineName)
        {
        }
    }
}