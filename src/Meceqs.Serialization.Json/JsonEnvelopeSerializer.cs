using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Meceqs.Serialization.Json
{
    public class JsonEnvelopeSerializer : IEnvelopeSerializer
    {
        private const string _contentType = "application/json";

        public string ContentType => _contentType;

        private static readonly JsonSerializerSettings _defaultSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        public Envelope Deserialize(Stream serializedEnvelope, Type envelopeType)
        {
            using (StreamReader reader = new StreamReader(serializedEnvelope, Encoding.UTF8))
            {
                string json = reader.ReadToEnd();
                return (Envelope)JsonConvert.DeserializeObject(json, envelopeType, _defaultSettings);
            }
        }

        public byte[] Serialize(Envelope envelope)
        {
            string json = JsonConvert.SerializeObject(envelope);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}