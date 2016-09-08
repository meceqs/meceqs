using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureEventHubs.Internal
{
    public interface IEventDataConverter
    {
        EventData ConvertToEventData(Envelope envelope);

        Envelope ConvertToEnvelope(EventData eventData);
    }
}