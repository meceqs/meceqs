using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.Transport.AzureServiceBus
{
    public interface IServiceBusConsumer
    {
        Task ConsumeAsync(BrokeredMessage brokeredMessage, CancellationToken cancellation);
    }
}