using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;

namespace Meceqs.AzureEventHubs.Receiving
{
    public interface IEventHubReceiver
    {
        Task ReceiveAsync(EventData eventData, CancellationToken cancellation);
    }
}
