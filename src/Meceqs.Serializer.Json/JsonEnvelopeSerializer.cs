using System;
using Newtonsoft.Json;

namespace Meceqs.Serializer.Json
{
    public class JsonEnvelopeSerializer : IEnvelopeSerializer
    {
        private static readonly JsonSerializerSettings _defaultSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public Envelope Deserialize(string serializedEnvelope, Type envelopeType)
        {
            return (Envelope)JsonConvert.DeserializeObject(serializedEnvelope, envelopeType, _defaultSettings);
        }

        public string Serialize(Envelope envelope)
        {
            return JsonConvert.SerializeObject(envelope);
        }
    }
}