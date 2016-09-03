using System.IO;

namespace Meceqs.Serialization
{
    public interface IEnvelopeDeserializer
    {
        Envelope DeserializeFromStream(Stream serializedEnvelope, string contentType, string messageType);
    }
}