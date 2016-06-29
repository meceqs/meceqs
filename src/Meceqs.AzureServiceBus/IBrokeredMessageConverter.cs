using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureServiceBus
{
    public interface IBrokeredMessageConverter
    {
        Envelope ConvertToEnvelope(BrokeredMessage brokeredMessage);
    }
}