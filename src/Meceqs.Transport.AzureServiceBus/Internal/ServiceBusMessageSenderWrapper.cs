using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.Transport.AzureServiceBus.Internal
{
    public class ServiceBusMessageSenderWrapper : IServiceBusMessageSender
    {
        private readonly MessageSender _sender;

        public ServiceBusMessageSenderWrapper(MessageSender sender)
        {
            Check.NotNull(sender, nameof(sender));

            _sender = sender;
        }

        public Task SendAsync(BrokeredMessage message)
        {
            return _sender.SendAsync(message);
        }

        public void Close()
        {
            _sender.Close();
        }
    }
}