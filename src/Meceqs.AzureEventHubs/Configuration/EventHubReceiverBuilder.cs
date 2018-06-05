using Meceqs.AzureEventHubs.Receiving;
using Meceqs.Configuration;
using Meceqs.Transport;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.AzureEventHubs.Configuration
{
    public class EventHubReceiverBuilder : TransportReceiverBuilder<IEventHubReceiverBuilder, EventHubReceiverOptions>,
        IEventHubReceiverBuilder
    {
        public override IEventHubReceiverBuilder Instance => this;

        public EventHubReceiverBuilder(IMeceqsBuilder meceqsBuilder, string pipelineName)
            : base(meceqsBuilder, pipelineName)
        {
        }
    }
}