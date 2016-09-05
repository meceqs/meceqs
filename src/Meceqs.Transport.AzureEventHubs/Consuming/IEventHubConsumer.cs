using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.Transport.AzureEventHubs.Consuming
{
    public interface IEventHubConsumer
    {
        Task ConsumeAsync(EventData eventData, CancellationToken cancellation);
    }
}