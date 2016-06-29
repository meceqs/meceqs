using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureServiceBus
{
    public interface IBrokeredMessageHandler
    {
        Task HandleAsync(BrokeredMessage brokeredMessage, CancellationToken cancellation);
    }
}