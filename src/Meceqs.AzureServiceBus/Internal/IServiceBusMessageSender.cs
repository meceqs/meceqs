using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace Meceqs.AzureServiceBus.Internal
{
    public interface IServiceBusMessageSender
    {
        Task SendAsync(Message message);

        void Close();
    }
}
