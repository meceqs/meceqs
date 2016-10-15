using Meceqs.Transport;

namespace Meceqs.AzureServiceBus.Configuration
{
    public class ServiceBusConsumerBuilder : TransportConsumerBuilder<IServiceBusConsumerBuilder, ServiceBusConsumerOptions>,
        IServiceBusConsumerBuilder
    {
        public override IServiceBusConsumerBuilder Instance => this;
    }
}