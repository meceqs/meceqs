using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.Transport.AzureServiceBus.Internal
{
    public interface IServiceBusMessageSender
    {
        Task SendAsync(BrokeredMessage message);

        void Close();
    }
}