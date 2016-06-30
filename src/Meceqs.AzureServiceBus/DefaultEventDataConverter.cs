using System;
using System.IO;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureServiceBus
{
    public class DefaultEventDataConverter : IEventDataConverter
    {
        

        private readonly IEnvelopeTypeConverter _envelopeTypeConverter;
        private readonly IEnvelopeSerializer _envelopeSerializer;

        public DefaultEventDataConverter(IEnvelopeTypeConverter envelopeTypeConverter, IEnvelopeSerializer envelopeSerializer)
        {
            Check.NotNull(envelopeTypeConverter, nameof(envelopeTypeConverter));
            Check.NotNull(envelopeSerializer, nameof(envelopeSerializer));

            _envelopeTypeConverter = envelopeTypeConverter;
            _envelopeSerializer = envelopeSerializer;
        }

        public EventData ConvertToEventData(Envelope envelope)
        {
            Check.NotNull(envelope, nameof(envelope));

            byte[] serializedEnvelope = _envelopeSerializer.Serialize(envelope);

            EventData eventData = new EventData(serializedEnvelope);

            eventData.Properties[TransportHeaderNames.ContentType] = _envelopeSerializer.ContentType;

            eventData.Properties[TransportHeaderNames.MessageId] = envelope.MessageId;
            eventData.Properties[TransportHeaderNames.MessageName] = envelope.MessageName;
            eventData.Properties[TransportHeaderNames.MessageType] = envelope.MessageType;

            return eventData;
        }

        public Envelope ConvertToEnvelope(EventData eventData)
        {
            Check.NotNull(eventData, nameof(eventData));

            // TODO @cweiss validations?
            string messageType = (string)eventData.Properties[TransportHeaderNames.MessageType];
            Type envelopeType = _envelopeTypeConverter.ConvertToEnvelopeType(messageType);

            Stream serializedEnvelope = eventData.GetBodyStream();

            return _envelopeSerializer.Deserialize(serializedEnvelope, envelopeType);
        }
    }
}