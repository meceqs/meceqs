using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Meceqs.Serialization.Json
{
    public class JsonEnvelopeDeserializer : IEnvelopeDeserializer, IResultDeserializer
    {
        private const string ContentType = "application/json";

        private readonly IEnvelopeTypeLoader _envelopeTypeLoader;

        public JsonEnvelopeDeserializer(IEnvelopeTypeLoader envelopetypeLoader)
        {
            Check.NotNull(envelopetypeLoader, nameof(envelopetypeLoader));

            _envelopeTypeLoader = envelopetypeLoader;
        }

        public object DeserializeResultFromStream(string contentType, Stream serializedResult, Type resultType)
        {
            if (!string.Equals(contentType, ContentType, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Invalid ContentType! Expected: {ContentType}; Actual: {contentType}");
            }

            using (StreamReader reader = new StreamReader(serializedResult, Encoding.UTF8))
            {
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject(json, resultType);
            }
        }

        public Envelope DeserializeEnvelopeFromStream(string contentType, Stream serializedEnvelope, string messageType)
        {
            if (!string.Equals(contentType, ContentType, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Invalid ContentType! Expected: {ContentType}; Actual: {contentType}");
            }

            Type envelopeType = _envelopeTypeLoader.LoadEnvelopeType(messageType);

            using (StreamReader reader = new StreamReader(serializedEnvelope, Encoding.UTF8))
            {
                string json = reader.ReadToEnd();
                return (Envelope)JsonConvert.DeserializeObject(json, envelopeType);
            }
        }
    }
}