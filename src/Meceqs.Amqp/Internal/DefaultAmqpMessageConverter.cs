using Amqp;
using Amqp.Framing;
using Meceqs.Serialization;
using Meceqs.Transport;

namespace Meceqs.Amqp.Internal
{
    public class DefaultAmqpMessageConverter : IAmqpMessageConverter
    {
        private readonly IEnvelopeSerializer _serializer;
        private readonly IEnvelopeDeserializer _deserializer;

        public DefaultAmqpMessageConverter(IEnvelopeSerializer serializer, IEnvelopeDeserializer deserializer)
        {
            Check.NotNull(serializer, nameof(serializer));
            Check.NotNull(deserializer, nameof(deserializer));

            _serializer = serializer;
            _deserializer = deserializer;
        }

        public Message ConvertToAmqpMessage(Envelope envelope)
        {
            Check.NotNull(envelope, nameof(envelope));

            byte[] serializedEnvelope = _serializer.SerializeEnvelopeToByteArray(envelope);

            Message amqpMessage = new Message();

            amqpMessage.BodySection = new Data { Binary = serializedEnvelope };
            amqpMessage.Properties = new Properties();
            amqpMessage.ApplicationProperties = new ApplicationProperties();

            // Some properties are written to the object and to the headers-dictionary
            // to be consistent with other transports that only have header-dictionaries.

            amqpMessage.Properties.ContentType = _serializer.ContentType;
            amqpMessage.ApplicationProperties[TransportHeaderNames.ContentType] = _serializer.ContentType;

            amqpMessage.ApplicationProperties[TransportHeaderNames.MessageType] = envelope.MessageType;

            amqpMessage.Properties.MessageId = envelope.MessageId.ToString();
            amqpMessage.ApplicationProperties[TransportHeaderNames.MessageId] = envelope.MessageId;

            amqpMessage.Properties.CorrelationId = envelope.CorrelationId.ToString();

            return amqpMessage;
        }

        public Envelope ConvertToEnvelope(Message amqpMessage)
        {
            Check.NotNull(amqpMessage, nameof(amqpMessage));

            // TODO @cweiss validations?
            string contentType = amqpMessage.Properties.ContentType ?? (string)amqpMessage.ApplicationProperties[TransportHeaderNames.ContentType];
            string messageType = (string)amqpMessage.ApplicationProperties[TransportHeaderNames.MessageType];

            byte[] serializedEnvelope = amqpMessage.GetBody<byte[]>();

            return _deserializer.DeserializeEnvelope(contentType, serializedEnvelope, messageType);
        }
    }
}