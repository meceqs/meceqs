using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;

namespace Meceqs.AzureEventHubs.Internal
{
    public interface IEventHubClient
    {
        Task SendAsync(EventData data, string partitionKey);

        void Close();
    }
}