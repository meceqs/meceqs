using System;
using System.IO;
using Meceqs.Serialization;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureServiceBus
{
    public class DefaultBrokeredMessageConverter : IBrokeredMessageConverter
    {
        private readonly IEnvelopeSerializer _envelopeSerializer;
        private readonly IEnvelopeTypeLoader _envelopeTypeLoader;

        public DefaultBrokeredMessageConverter(IEnvelopeSerializer envelopeSerializer, IEnvelopeTypeLoader envelopeTypeLoader)
        {
            Check.NotNull(envelopeSerializer, nameof(envelopeSerializer));
            Check.NotNull(envelopeTypeLoader, nameof(envelopeTypeLoader));

            _envelopeSerializer = envelopeSerializer;
            _envelopeTypeLoader = envelopeTypeLoader;
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
            string messageType = (string)brokeredMessage.Properties[TransportHeaderNames.MessageType];
            Type envelopeType = _envelopeTypeLoader.LoadEnvelopeType(messageType);

            Stream serializedEnvelope = brokeredMessage.GetBody<Stream>();

            return _envelopeSerializer.Deserialize(serializedEnvelope, envelopeType);
        }
    }
}