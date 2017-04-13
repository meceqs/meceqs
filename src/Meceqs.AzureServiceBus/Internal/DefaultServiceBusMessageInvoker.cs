using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace Meceqs.AzureServiceBus.Internal
{
    public class DefaultServiceBusMessageInvoker : IServiceBusMessageInvoker
    {
        public Task AbandonAsync(Message message)
        {
            return message.AbandonAsync();
        }

        public Task CompleteAsync(Message message)
        {
            return message.CompleteAsync();
        }
    }
}