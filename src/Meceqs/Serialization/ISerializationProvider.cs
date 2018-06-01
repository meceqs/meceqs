using System.Collections.Generic;
using System.IO;

namespace Meceqs.Serialization
{
    public interface ISerializationProvider
    {
        ISerializer GetDefaultSerializer();

        bool TryGetSerializer(string contentType, out ISerializer serializer);

        ISerializer GetSerializer(IEnumerable<string> supportedContentTypes);

        Envelope DeserializeEnvelope(string contentType, byte[] serializedEnvelope, string messageType);

        Envelope DeserializeEnvelope(string contentType, Stream serializedEnvelope, string messageType);
    }
}
