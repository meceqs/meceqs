using Microsoft.ServiceBus.Messaging;

namespace Meceqs.Transport.AzureServiceBus.Internal
{
    public interface IBrokeredMessageConverter
    {
        BrokeredMessage ConvertToBrokeredMessage(Envelope envelope);

        Envelope ConvertToEnvelope(BrokeredMessage brokeredMessage);
    }
}