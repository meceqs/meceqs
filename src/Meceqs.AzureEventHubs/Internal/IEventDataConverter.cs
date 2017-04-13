using Microsoft.Azure.EventHubs;

namespace Meceqs.AzureEventHubs.Internal
{
    public interface IEventDataConverter
    {
        EventData ConvertToEventData(Envelope envelope);

        Envelope ConvertToEnvelope(EventData eventData);
    }
}