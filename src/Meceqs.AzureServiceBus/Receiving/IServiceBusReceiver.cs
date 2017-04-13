using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace Meceqs.AzureServiceBus.Receiving
{
    public interface IServiceBusReceiver
    {
        Task ReceiveAsync(Message message, CancellationToken cancellation);
    }
}