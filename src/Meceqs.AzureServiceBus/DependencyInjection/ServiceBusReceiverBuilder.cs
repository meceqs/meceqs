using Meceqs.AzureServiceBus.Receiving;
using Meceqs.Transport;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.AzureServiceBus.DependencyInjection
{
    public class ServiceBusReceiverBuilder : TransportReceiverBuilder<IServiceBusReceiverBuilder, ServiceBusReceiverOptions>,
        IServiceBusReceiverBuilder
    {
        public override IServiceBusReceiverBuilder Instance => this;

        public ServiceBusReceiverBuilder(IMeceqsBuilder meceqsBuilder, string pipelineName)
            : base(meceqsBuilder, pipelineName)
        {
        }
    }
}