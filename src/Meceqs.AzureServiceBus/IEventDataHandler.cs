using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureServiceBus
{
    public interface IEventDataHandler
    {
        Task HandleAsync(EventData eventData, CancellationToken cancellation);
    }
}