using System;
using System.IO;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureServiceBus
{
    public class DefaultBrokeredMessageConverter : IBrokeredMessageConverter
    {
        private readonly IEnvelopeTypeConverter _envelopeTypeConverter;
        private readonly IEnvelopeSerializer _envelopeSerializer;

        public DefaultBrokeredMessageConverter(IEnvelopeTypeConverter envelopeTypeConverter, IEnvelopeSerializer envelopeSerializer)
        {
            Check.NotNull(envelopeTypeConverter, nameof(envelopeTypeConverter));
            Check.NotNull(envelopeSerializer, nameof(envelopeSerializer));

            _envelopeTypeConverter = envelopeTypeConverter;
            _envelopeSerializer = envelopeSerializer;
        }

        public BrokeredMessage ConvertToBrokeredMessage(Envelope envelope)
        {
            Check.NotNull(envelope, nameof(envelope));

            byte[] serializedEnvelope = _envelopeSerializer.Serialize(envelope);
            MemoryStream payloadStream = new MemoryStream(serializedEnvelope);

            BrokeredMessage brokeredMessage = new BrokeredMessage(payloadStream, ownsStream: true);

            // Content-Type is written to both locations to be consistent with other transports that only have header-dictionaries.
            brokeredMessage.ContentType = _envelopeSerializer.ContentType;
            brokeredMessage.Properties[TransportHeaderNames.ContentType] = _envelopeSerializer.ContentType;

            brokeredMessage.Properties[TransportHeaderNames.MessageId] = envelope.MessageId;
            brokeredMessage.Properties[TransportHeaderNames.MessageName] = envelope.MessageName;
            brokeredMessage.Properties[TransportHeaderNames.MessageType] = envelope.MessageType;

            return brokeredMessage;
        }

        public Envelope ConvertToEnvelope(BrokeredMessage brokeredMessage)
        {
            Check.NotNull(brokeredMessage, nameof(brokeredMessage));

            // TODO @cweiss validations?
            string messageType = brokeredMessage.ContentType;
            Type envelopeType = _envelopeTypeConverter.ConvertToEnvelopeType(messageType);

            Stream serializedEnvelope = brokeredMessage.GetBody<Stream>();

            return _envelopeSerializer.Deserialize(serializedEnvelope, envelopeType);
        }
    }
}