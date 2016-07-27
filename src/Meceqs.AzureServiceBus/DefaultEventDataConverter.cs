using System;
using System.IO;
using Meceqs.Handling;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureServiceBus
{
    public class DefaultEventDataConverter : IEventDataConverter
    {


        private readonly IEnvelopeSerializer _envelopeSerializer;
        private readonly IEnvelopeTypeLoader _envelopeTypeLoader;

        public DefaultEventDataConverter(IEnvelopeSerializer envelopeSerializer, IEnvelopeTypeLoader envelopeTypeLoader)
        {
            Check.NotNull(envelopeSerializer, nameof(envelopeSerializer));
            Check.NotNull(envelopeTypeLoader, nameof(envelopeTypeLoader));

            _envelopeSerializer = envelopeSerializer;
            _envelopeTypeLoader = envelopeTypeLoader;
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
            Type envelopeType = _envelopeTypeLoader.LoadEnvelopeType(messageType);

            Stream serializedEnvelope = eventData.GetBodyStream();

            return _envelopeSerializer.Deserialize(serializedEnvelope, envelopeType);
        }
    }
}