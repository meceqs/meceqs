using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureEventHubs.Internal
{
    public interface IEventHubClient
    {
        Task SendAsync(EventData data);

        void Close();
    }
}