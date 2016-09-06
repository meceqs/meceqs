using Microsoft.ServiceBus.Messaging;

namespace Meceqs.Transport.AzureServiceBus.Internal
{
    public class DefaultServiceBusMessageSenderFactory : IServiceBusMessageSenderFactory
    {
        public IServiceBusMessageSender CreateMessageSender(string connectionString, string entityPath)
        {
            Check.NotNullOrWhiteSpace(connectionString, nameof(connectionString));
            Check.NotNullOrWhiteSpace(entityPath, nameof(entityPath));

            var messagingFactory = MessagingFactory.CreateFromConnectionString(connectionString);
            var messageSender = messagingFactory.CreateMessageSender(entityPath);

            return new ServiceBusMessageSenderWrapper(messageSender);
        }
    }
}