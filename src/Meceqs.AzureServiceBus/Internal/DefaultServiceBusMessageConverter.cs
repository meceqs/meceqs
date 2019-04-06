using System;
using System.IO;
using Meceqs.Internal;
using Meceqs.Serialization;
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

            ISerializer serializer = _serializationProvider.GetSerializer(envelope.GetType());

            byte[] serializedEnvelope = serializer.SerializeToByteArray(envelope);

            Message serviceBusMessage = new Message(serializedEnvelope);

            serviceBusMessage.ContentType = serializer.ContentType;
            serviceBusMessage.MessageId = envelope.MessageId.ToString();
            serviceBusMessage.Label = envelope.MessageType;
            serviceBusMessage.CorrelationId = envelope.CorrelationId.ToString();

            return serviceBusMessage;
        }

        public Envelope ConvertToEnvelope(Message serviceBusMessage)
        {
            Guard.NotNull(serviceBusMessage, nameof(serviceBusMessage));

            string contentType = serviceBusMessage.ContentType ?? throw new InvalidOperationException("ContentType not set.");
            string messageType = serviceBusMessage.Label ?? throw new InvalidOperationException("Label not set.");

            Type envelopeType = _envelopeTypeLoader.LoadEnvelopeType(messageType);
            ISerializer serializer = _serializationProvider.GetSerializer(envelopeType, contentType);

            using (var envelopeStream = new MemoryStream(serviceBusMessage.Body))
            {
                return (Envelope)serializer.Deserialize(envelopeType, envelopeStream);
            }
        }
    }
}
