using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.Transport.AzureEventHubs.Internal
{
    public interface IEventHubClient
    {
        Task SendAsync(EventData data);

        void Close();
    }
}