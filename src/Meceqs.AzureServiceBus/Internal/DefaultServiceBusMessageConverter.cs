using Meceqs.Serialization;
using Meceqs.Transport;
using Microsoft.Azure.ServiceBus;

namespace Meceqs.AzureServiceBus.Internal
{
    public class DefaultServiceBusMessageConverter : IServiceBusMessageConverter
    {
        private readonly IEnvelopeSerializer _serializer;
        private readonly IEnvelopeDeserializer _deserializer;

        public DefaultServiceBusMessageConverter(IEnvelopeSerializer serializer, IEnvelopeDeserializer deserializer)
        {
            Guard.NotNull(serializer, nameof(serializer));
            Guard.NotNull(deserializer, nameof(deserializer));

            _serializer = serializer;
            _deserializer = deserializer;
        }

        public Message ConvertToServiceBusMessage(Envelope envelope)
        {
            Guard.NotNull(envelope, nameof(envelope));

            byte[] serializedEnvelope = _serializer.SerializeEnvelopeToByteArray(envelope);

            Message serviceBusMessage = new Message(serializedEnvelope);

            // Some properties are written to the object and to the headers-dictionary
            // to be consistent with other transports that only have header-dictionaries.

            serviceBusMessage.ContentType = _serializer.ContentType;
            serviceBusMessage.UserProperties[TransportHeaderNames.ContentType] = _serializer.ContentType;

            serviceBusMessage.MessageId = envelope.MessageId.ToString();
            serviceBusMessage.UserProperties[TransportHeaderNames.MessageId] = envelope.MessageId;
            serviceBusMessage.UserProperties[TransportHeaderNames.MessageType] = envelope.MessageType;

            serviceBusMessage.CorrelationId = envelope.CorrelationId.ToString();

            return serviceBusMessage;
        }

        public Envelope ConvertToEnvelope(Message serviceBusMessage)
        {
            Guard.NotNull(serviceBusMessage, nameof(serviceBusMessage));

            // TODO @cweiss validations?
            string contentType = serviceBusMessage.ContentType ?? (string)serviceBusMessage.UserProperties[TransportHeaderNames.ContentType];
            string messageType = (string)serviceBusMessage.UserProperties[TransportHeaderNames.MessageType];

            byte[] serializedEnvelope = serviceBusMessage.Body;

            return _deserializer.DeserializeEnvelope(contentType, serializedEnvelope, messageType);
        }
    }
}