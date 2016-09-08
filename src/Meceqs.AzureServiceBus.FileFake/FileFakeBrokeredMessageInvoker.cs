using System.Threading.Tasks;
using Meceqs.AzureServiceBus.Internal;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureServiceBus.FileFake
{
    public class FileFakeBrokeredMessageInvoker : IBrokeredMessageInvoker
    {
        public Task AbandonAsync(BrokeredMessage message)
        {
            return Task.CompletedTask;
        }

        public Task CompleteAsync(BrokeredMessage message)
        {
            return Task.CompletedTask;
        }
    }
}