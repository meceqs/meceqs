using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureServiceBus
{
    public interface IBrokeredMessageConverter
    {
        BrokeredMessage ConvertToBrokeredMessage(Envelope envelope);

        Envelope ConvertToEnvelope(BrokeredMessage brokeredMessage);
    }
}