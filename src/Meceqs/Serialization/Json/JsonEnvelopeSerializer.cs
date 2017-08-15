using System.Text;
using Newtonsoft.Json;

namespace Meceqs.Serialization.Json
{
    public class JsonEnvelopeSerializer : IEnvelopeSerializer, IResultSerializer
    {
        private const string _contentType = "application/json";

        private readonly JsonSerializerSettings _settings;

        public string ContentType => _contentType;

        public JsonEnvelopeSerializer(JsonSerializerSettings settings = null)
        {
            _settings = settings ?? JsonDefaults.DefaultSerializerSettings;
        }

        public byte[] SerializeEnvelopeToByteArray(Envelope envelope)
        {
            string json = SerializeEnvelopeToString(envelope);
            return Encoding.UTF8.GetBytes(json);
        }

        public string SerializeEnvelopeToString(Envelope envelope)
        {
            return SerializeResultToString((object)envelope);
        }

        public string SerializeResultToString(object result)
        {
            return JsonConvert.SerializeObject(result, _settings);

            // https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/JsonConvert.cs
            // (We don't use JsonConvert.SerializeObject because this always creates a new JsonSerializer)

            // TODO @cweiss Json Perf??
            // StringBuilder sb = new StringBuilder(256);
            // StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture);
            // using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
            // {
            //     jsonWriter.Formatting = _jsonSerializer.Formatting;

            //     _jsonSerializer.Serialize(jsonWriter, envelope, null);
            // }

            // return sw.ToString();
        }
    }
}