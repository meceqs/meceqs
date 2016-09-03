using System.IO;

namespace Meceqs.Serialization
{
    public interface IEnvelopeDeserializer
    {
        Envelope DeserializeFromStream(string contentType, Stream serializedEnvelope, string messageType);
    }
}