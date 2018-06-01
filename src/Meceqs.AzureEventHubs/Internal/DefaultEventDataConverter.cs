using Meceqs.Serialization;
using Meceqs.Transport;
using Microsoft.Azure.EventHubs;

namespace Meceqs.AzureEventHubs.Internal
{
    public class DefaultEventDataConverter : IEventDataConverter
    {
        private readonly ISerializationProvider _serializationProvider;

        public DefaultEventDataConverter(ISerializationProvider serializationProvider)
        {
            Guard.NotNull(serializationProvider, nameof(serializationProvider));

            _serializationProvider = serializationProvider;
        }

        public EventData ConvertToEventData(Envelope envelope)
        {
            Guard.NotNull(envelope, nameof(envelope));

            ISerializer serializer = _serializationProvider.GetDefaultSerializer();

            byte[] serializedEnvelope = serializer.SerializeToByteArray(envelope);

            EventData eventData = new EventData(serializedEnvelope);

            eventData.Properties[TransportHeaderNames.ContentType] = serializer.ContentType;

            eventData.Properties[TransportHeaderNames.MessageId] = envelope.MessageId;
            eventData.Properties[TransportHeaderNames.MessageType] = envelope.MessageType;

            return eventData;
        }

        public Envelope ConvertToEnvelope(EventData eventData)
        {
            Guard.NotNull(eventData, nameof(eventData));

            string contentType = (string)eventData.Properties[TransportHeaderNames.ContentType];
            string messageType = (string)eventData.Properties[TransportHeaderNames.MessageType];

            byte[] serializedEnvelope = eventData.Body.Array;

            return _serializationProvider.DeserializeEnvelope(contentType, serializedEnvelope, messageType);
        }
    }
}
