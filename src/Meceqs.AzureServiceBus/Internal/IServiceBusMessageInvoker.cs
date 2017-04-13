using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace Meceqs.AzureServiceBus.Internal
{
    public interface IServiceBusMessageInvoker
    {
        Task CompleteAsync(Message message);

        Task AbandonAsync(Message message);
    }
}