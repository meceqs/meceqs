using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Meceqs.Serialization.NewtonsoftJson
{
    public class NewtonsoftJsonSerializer : ISerializer
    {
        private readonly JsonSerializer _jsonSerializer;

        public string ContentType { get; } = "application/json";

        public NewtonsoftJsonSerializer(JsonSerializerSettings settings = null)
        {
            _jsonSerializer = JsonSerializer.Create(settings ?? NewtonsoftJsonDefaults.DefaultSerializerSettings);

        }

        public bool CanSerializeType(Type objectType)
        {
            return true;
        }

        public byte[] SerializeToByteArray(object obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                SerializeToStream(obj, memoryStream);
                return memoryStream.ToArray();
            }
        }

        public void SerializeToStream(object obj, Stream stream)
        {
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
            {
                _jsonSerializer.Serialize(writer, obj);
            }
        }

        public object Deserialize(Type objectType, byte[] serializedObject)
        {
            Guard.NotNull(objectType, nameof(objectType));

            using (var memoryStream = new MemoryStream(serializedObject))
            {
                return Deserialize(objectType, memoryStream);
            }
        }

        public object Deserialize(Type objectType, Stream serializedObject)
        {
            Guard.NotNull(objectType, nameof(objectType));

            using (StreamReader reader = new StreamReader(serializedObject, Encoding.UTF8))
            {
                return _jsonSerializer.Deserialize(reader, objectType);
            }
        }
    }
}
