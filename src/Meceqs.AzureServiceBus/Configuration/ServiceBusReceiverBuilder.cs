using Meceqs.Transport;

namespace Meceqs.AzureServiceBus.Configuration
{
    public class ServiceBusReceiverBuilder : TransportReceiverBuilder<IServiceBusReceiverBuilder, ServiceBusReceiverOptions>,
        IServiceBusReceiverBuilder
    {
        public override IServiceBusReceiverBuilder Instance => this;
    }
}