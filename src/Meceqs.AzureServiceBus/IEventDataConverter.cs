using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureServiceBus
{
    public interface IEventDataConverter
    {
        EventData ConvertToEventData(Envelope envelope);

        Envelope ConvertToEnvelope(EventData eventData);
    }
}