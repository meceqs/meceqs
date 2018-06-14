using Meceqs.AzureServiceBus.Sending;
using Meceqs.Transport;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface IServiceBusSenderBuilder : ITransportSenderBuilder<IServiceBusSenderBuilder, ServiceBusSenderOptions>
    {
        IServiceBusSenderBuilder SetConnectionString(string connectionString);
    }
}