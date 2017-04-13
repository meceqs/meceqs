using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace Meceqs.AzureServiceBus.Internal
{
    public class ServiceBusMessageSenderWrapper : IServiceBusMessageSender
    {
        private readonly MessageSender _sender;

        public ServiceBusMessageSenderWrapper(MessageSender sender)
        {
            Guard.NotNull(sender, nameof(sender));

            _sender = sender;
        }

        public Task SendAsync(Message message)
        {
            return _sender.SendAsync(message);
        }

        public void Close()
        {
            _sender.Close();
        }
    }
}