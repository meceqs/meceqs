using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureServiceBus.Internal
{
    public interface IBrokeredMessageInvoker
    {
        Task CompleteAsync(BrokeredMessage message);

        Task AbandonAsync(BrokeredMessage message);
    }
}