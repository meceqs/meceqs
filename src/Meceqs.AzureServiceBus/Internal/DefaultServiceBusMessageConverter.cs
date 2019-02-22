using System;
using System.IO;
using Meceqs.Internal;
using Meceqs.Serialization;
using Meceqs.Transport;
using Microsoft.Azure.ServiceBus;

namespace Meceqs.AzureServiceBus.Internal
{
    public class DefaultServiceBusMessageConverter : IServiceBusMessageConverter
    {
        private readonly ISerializationProvider _serializationProvider;
        private readonly IEnvelopeTypeLoader _envelopeTypeLoader;

        public DefaultServiceBusMessageConverter(ISerializationProvider serializationProvider, IEnvelopeTypeLoader envelopeTypeLoader)
        {
            Guard.NotNull(serializationProvider, nameof(serializationProvider));
            Guard.NotNull(envelopeTypeLoader, nameof(envelopeTypeLoader));

            _serializationProvider = serializationProvider;
            _envelopeTypeLoader = envelopeTypeLoader;
        }

        public Message ConvertToServiceBusMessage(Envelope envelope)
        {
            Guard.NotNull(envelope, nameof(envelope));

            ISerializer serializer = _serializationProvider.GetSerializer(envelope.Message.GetType());

            byte[] serializedEnvelope = serializer.SerializeToByteArray(envelope);

            Message serviceBusMessage = new Message(serializedEnvelope);

            // Some properties are written to the object and to the headers-dictionary
            // to be consistent with other transports that only have header-dictionaries.

            serviceBusMessage.ContentType = serializer.ContentType;
            serviceBusMessage.UserProperties[TransportHeaderNames.ContentType] = serializer.ContentType;

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

            Type envelopeType = _envelopeTypeLoader.LoadEnvelopeType(messageType);

            using (var envelopeStream = new MemoryStream(serviceBusMessage.Body))
            {
                return (Envelope)_serializationProvider.Deserialize(contentType, envelopeType, envelopeStream);
            }
        }
    }
}
