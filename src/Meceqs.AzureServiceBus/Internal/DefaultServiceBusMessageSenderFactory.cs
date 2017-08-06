using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace Meceqs.AzureServiceBus.Internal
{
    public class DefaultServiceBusMessageSenderFactory : IServiceBusMessageSenderFactory
    {
        public IServiceBusMessageSender CreateMessageSender(string connectionString)
        {
            Guard.NotNullOrWhiteSpace(connectionString, nameof(connectionString));

            var connectionBuilder = new ServiceBusConnectionStringBuilder(connectionString);
            var messageSender = new MessageSender(connectionBuilder);

            return new ServiceBusMessageSenderWrapper(messageSender);
        }
    }
}