using System.IO;
using Meceqs.Serialization;
using Meceqs.Transport;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureServiceBus.Internal
{
    public class DefaultBrokeredMessageConverter : IBrokeredMessageConverter
    {
        private readonly IEnvelopeSerializer _serializer;
        private readonly IEnvelopeDeserializer _deserializer;

        public DefaultBrokeredMessageConverter(IEnvelopeSerializer serializer, IEnvelopeDeserializer deserializer)
        {
            Check.NotNull(serializer, nameof(serializer));
            Check.NotNull(deserializer, nameof(deserializer));

            _serializer = serializer;
            _deserializer = deserializer;
        }

        public BrokeredMessage ConvertToBrokeredMessage(Envelope envelope)
        {
            Check.NotNull(envelope, nameof(envelope));

            byte[] serializedEnvelope = _serializer.SerializeToByteArray(envelope);
            MemoryStream payloadStream = new MemoryStream(serializedEnvelope);

            BrokeredMessage brokeredMessage = new BrokeredMessage(payloadStream, ownsStream: true);

            // Some properties are written to the object and to the headers-dictionary
            // to be consistent with other transports that only have header-dictionaries.

            brokeredMessage.ContentType = _serializer.ContentType;
            brokeredMessage.Properties[TransportHeaderNames.ContentType] = _serializer.ContentType;

            brokeredMessage.MessageId = envelope.MessageId.ToString();
            brokeredMessage.Properties[TransportHeaderNames.MessageId] = envelope.MessageId;

            brokeredMessage.Properties[TransportHeaderNames.MessageName] = envelope.MessageName;
            brokeredMessage.Properties[TransportHeaderNames.MessageType] = envelope.MessageType;

            brokeredMessage.CorrelationId = envelope.CorrelationId.ToString();

            return brokeredMessage;
        }

        public Envelope ConvertToEnvelope(BrokeredMessage brokeredMessage)
        {
            Check.NotNull(brokeredMessage, nameof(brokeredMessage));

            // TODO @cweiss validations?
            string contentType = brokeredMessage.ContentType ?? (string)brokeredMessage.Properties[TransportHeaderNames.ContentType];
            string messageType = (string)brokeredMessage.Properties[TransportHeaderNames.MessageType];

            Stream serializedEnvelope = brokeredMessage.GetBody<Stream>();

            return _deserializer.DeserializeFromStream(contentType, serializedEnvelope, messageType);
        }
    }
}