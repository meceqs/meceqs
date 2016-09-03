using System.IO;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.Transport.AzureEventHubs.Internal
{
    public class DefaultEventDataConverter : IEventDataConverter
    {
        private readonly IEnvelopeSerializer _serializer;
        private readonly IEnvelopeDeserializer _deserializer;

        public DefaultEventDataConverter(IEnvelopeSerializer serializer, IEnvelopeDeserializer deserializer)
        {
            Check.NotNull(serializer, nameof(serializer));
            Check.NotNull(deserializer, nameof(deserializer));

            _serializer = serializer;
            _deserializer = deserializer;
        }

        public EventData ConvertToEventData(Envelope envelope)
        {
            Check.NotNull(envelope, nameof(envelope));

            byte[] serializedEnvelope = _serializer.SerializeToByteArray(envelope);

            EventData eventData = new EventData(serializedEnvelope);

            eventData.Properties[TransportHeaderNames.ContentType] = _serializer.ContentType;

            eventData.Properties[TransportHeaderNames.MessageId] = envelope.MessageId;
            eventData.Properties[TransportHeaderNames.MessageName] = envelope.MessageName;
            eventData.Properties[TransportHeaderNames.MessageType] = envelope.MessageType;

            return eventData;
        }

        public Envelope ConvertToEnvelope(EventData eventData)
        {
            Check.NotNull(eventData, nameof(eventData));

            // TODO @cweiss validations?
            string contentType = (string)eventData.Properties[TransportHeaderNames.ContentType];
            string messageType = (string)eventData.Properties[TransportHeaderNames.MessageType];

            Stream serializedEnvelope = eventData.GetBodyStream();

            return _deserializer.DeserializeFromStream(contentType, serializedEnvelope, messageType);
        }
    }
}