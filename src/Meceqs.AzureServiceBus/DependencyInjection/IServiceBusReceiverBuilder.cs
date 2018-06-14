using Meceqs.AzureServiceBus.Receiving;
using Meceqs.Transport;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface IServiceBusReceiverBuilder : ITransportReceiverBuilder<IServiceBusReceiverBuilder, ServiceBusReceiverOptions>
    {
    }
}