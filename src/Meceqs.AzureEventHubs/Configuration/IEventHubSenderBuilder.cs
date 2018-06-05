using Meceqs.AzureEventHubs.Sending;
using Meceqs.Transport;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface IEventHubSenderBuilder : ITransportSenderBuilder<IEventHubSenderBuilder, EventHubSenderOptions>
    {
        IEventHubSenderBuilder SetConnectionString(string connectionString);
    }
}