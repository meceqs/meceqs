using Meceqs.Serialization;
using Meceqs.Transport;
using Microsoft.Azure.ServiceBus;

namespace Meceqs.AzureServiceBus.Internal
{
    public class DefaultServiceBusMessageConverter : IServiceBusMessageConverter
    {
        private readonly ISerializationProvider _serializationProvider;

        public DefaultServiceBusMessageConverter(ISerializationProvider serializationProvider)
        {
            Guard.NotNull(serializationProvider, nameof(serializationProvider));

            _serializationProvider = serializationProvider;
        }

        public Message ConvertToServiceBusMessage(Envelope envelope)
        {
            Guard.NotNull(envelope, nameof(envelope));

            ISerializer serializer = _serializationProvider.GetDefaultSerializer();

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

            byte[] serializedEnvelope = serviceBusMessage.Body;

            return _serializationProvider.DeserializeEnvelope(contentType, serializedEnvelope, messageType);
        }
    }
}
