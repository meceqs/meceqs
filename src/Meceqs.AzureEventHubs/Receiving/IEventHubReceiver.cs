using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureEventHubs.Receiving
{
    public interface IEventHubReceiver
    {
        Task ReceiveAsync(EventData eventData, CancellationToken cancellation);
    }
}