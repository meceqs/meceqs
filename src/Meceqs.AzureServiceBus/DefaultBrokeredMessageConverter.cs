using System;
using System.IO;
using System.Text;
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

            string serializedEnvelope = _envelopeSerializer.Serialize(envelope);
            MemoryStream payloadStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedEnvelope));

            BrokeredMessage brokeredMessage = new BrokeredMessage(payloadStream, ownsStream: true);

            // TODO @cweiss write used serializer into properties to allow consumers to select the correct deserializer.

            brokeredMessage.ContentType = envelope.MessageType;

            return brokeredMessage;
        }

        public Envelope ConvertToEnvelope(BrokeredMessage brokeredMessage)
        {
            Check.NotNull(brokeredMessage, nameof(brokeredMessage));

            // TODO @cweiss validations?
            string messageType = brokeredMessage.ContentType;
            Type envelopeType = _envelopeTypeConverter.ConvertToEnvelopeType(messageType);

            string serializedEnvelope = brokeredMessage.GetBody<string>();

            return _envelopeSerializer.Deserialize(serializedEnvelope, envelopeType);
        }
    }
}