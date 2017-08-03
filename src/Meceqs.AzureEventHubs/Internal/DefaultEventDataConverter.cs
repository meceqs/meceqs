using System.IO;
using Meceqs.Serialization;
using Meceqs.Transport;
using Microsoft.Azure.EventHubs;

namespace Meceqs.AzureEventHubs.Internal
{
    public class DefaultEventDataConverter : IEventDataConverter
    {
        private readonly IEnvelopeSerializer _serializer;
        private readonly IEnvelopeDeserializer _deserializer;

        public DefaultEventDataConverter(IEnvelopeSerializer serializer, IEnvelopeDeserializer deserializer)
        {
            Guard.NotNull(serializer, nameof(serializer));
            Guard.NotNull(deserializer, nameof(deserializer));

            _serializer = serializer;
            _deserializer = deserializer;
        }

        public EventData ConvertToEventData(Envelope envelope)
        {
            Guard.NotNull(envelope, nameof(envelope));

            byte[] serializedEnvelope = _serializer.SerializeEnvelopeToByteArray(envelope);

            EventData eventData = new EventData(serializedEnvelope);

            eventData.Properties[TransportHeaderNames.ContentType] = _serializer.ContentType;

            eventData.Properties[TransportHeaderNames.MessageId] = envelope.MessageId;
            eventData.Properties[TransportHeaderNames.MessageType] = envelope.MessageType;

            return eventData;
        }

        public Envelope ConvertToEnvelope(EventData eventData)
        {
            Guard.NotNull(eventData, nameof(eventData));

            // TODO @cweiss validations?
            string contentType = (string)eventData.Properties[TransportHeaderNames.ContentType];
            string messageType = (string)eventData.Properties[TransportHeaderNames.MessageType];

            byte[] serializedEnvelope = eventData.Body.Array;

            return _deserializer.DeserializeEnvelope(contentType, serializedEnvelope, messageType);
        }
    }
}