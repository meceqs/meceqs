using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureServiceBus.Receiving
{
    public interface IServiceBusReceiver
    {
        Task ReceiveAsync(BrokeredMessage brokeredMessage, CancellationToken cancellation);
    }
}