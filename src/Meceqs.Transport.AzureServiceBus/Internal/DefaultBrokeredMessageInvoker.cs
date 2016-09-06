using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.Transport.AzureServiceBus.Internal
{
    public class DefaultBrokeredMessageInvoker : IBrokeredMessageInvoker
    {
        public Task AbandonAsync(BrokeredMessage message)
        {
            return message.AbandonAsync();
        }

        public Task CompleteAsync(BrokeredMessage message)
        {
            return message.CompleteAsync();
        }
    }
}