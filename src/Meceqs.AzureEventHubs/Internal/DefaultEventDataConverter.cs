using System;
using System.IO;
using Meceqs.Internal;
using Meceqs.Serialization;
using Meceqs.Transport;
using Microsoft.Azure.EventHubs;

namespace Meceqs.AzureEventHubs.Internal
{
    public class DefaultEventDataConverter : IEventDataConverter
    {
        private readonly ISerializationProvider _serializationProvider;
        private readonly IEnvelopeTypeLoader _envelopeTypeLoader;

        public DefaultEventDataConverter(ISerializationProvider serializationProvider, IEnvelopeTypeLoader envelopeTypeLoader)
        {
            Guard.NotNull(serializationProvider, nameof(serializationProvider));
            Guard.NotNull(envelopeTypeLoader, nameof(envelopeTypeLoader));

            _serializationProvider = serializationProvider;
            _envelopeTypeLoader = envelopeTypeLoader;
        }

        public EventData ConvertToEventData(Envelope envelope)
        {
            Guard.NotNull(envelope, nameof(envelope));

            ISerializer serializer = _serializationProvider.GetSerializer(envelope.Message.GetType());

            byte[] serializedEnvelope = serializer.SerializeToByteArray(envelope);

            EventData eventData = new EventData(serializedEnvelope);

            eventData.Properties[TransportHeaderNames.ContentType] = serializer.ContentType;

            eventData.Properties[TransportHeaderNames.MessageId] = envelope.MessageId;
            eventData.Properties[TransportHeaderNames.MessageType] = envelope.MessageType;

            return eventData;
        }

        public Envelope ConvertToEnvelope(EventData eventData)
        {
            Guard.NotNull(eventData, nameof(eventData));

            string contentType = (string)eventData.Properties[TransportHeaderNames.ContentType];
            string messageType = (string)eventData.Properties[TransportHeaderNames.MessageType];

            Type envelopeType = _envelopeTypeLoader.LoadEnvelopeType(messageType);

            using (var envelopeStream = new MemoryStream(eventData.Body.Array))
            {
                return (Envelope)_serializationProvider.Deserialize(contentType, envelopeType, envelopeStream);
            }
        }
    }
}
