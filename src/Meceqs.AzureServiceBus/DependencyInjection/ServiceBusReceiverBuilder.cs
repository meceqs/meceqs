using Meceqs.AzureServiceBus.Receiving;
using Meceqs.Transport;

namespace Microsoft.Extensions.DependencyInjection
{
    public class ServiceBusReceiverBuilder : ReceiveTransportBuilder<ServiceBusReceiverBuilder, ServiceBusReceiveEndpointOptions>
    {
        protected override ServiceBusReceiverBuilder Instance => this;

        public ServiceBusReceiverBuilder(IMeceqsBuilder meceqsBuilder, string pipelineName)
            : base(meceqsBuilder, pipelineName)
        {
        }
    }
}