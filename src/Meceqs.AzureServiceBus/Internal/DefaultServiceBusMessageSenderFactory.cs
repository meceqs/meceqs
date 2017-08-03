using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureServiceBus.Internal
{
    public class DefaultServiceBusMessageSenderFactory : IServiceBusMessageSenderFactory
    {
        public IServiceBusMessageSender CreateMessageSender(string connectionString, string entityPath)
        {
            Guard.NotNullOrWhiteSpace(connectionString, nameof(connectionString));
            Guard.NotNullOrWhiteSpace(entityPath, nameof(entityPath));

            var messagingFactory = MessagingFactory.CreateFromConnectionString(connectionString);
            var messageSender = messagingFactory.CreateMessageSender(entityPath);

            return new ServiceBusMessageSenderWrapper(messageSender);
        }
    }
}