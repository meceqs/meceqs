namespace Meceqs.Transport.AzureServiceBus.Internal
{
    public interface IServiceBusMessageSenderFactory
    {
        IServiceBusMessageSender CreateMessageSender(string connectionString, string entityPath);
    }
}