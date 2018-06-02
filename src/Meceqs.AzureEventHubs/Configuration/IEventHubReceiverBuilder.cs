using Meceqs.Transport;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface IEventHubReceiverBuilder : ITransportReceiverBuilder<IEventHubReceiverBuilder>
    {
    }
}