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

        public byte[] SerializeToByteArray(Envelope envelope)
        {
            string json = SerializeToString(envelope);
            return Encoding.UTF8.GetBytes(json);
        }

        public string SerializeToString(Envelope envelope)
        {
            return JsonConvert.SerializeObject(envelope);
        }
    }
}