using System.IO;

namespace Meceqs.Serialization
{
    public interface IEnvelopeDeserializer
    {
        Envelope DeserializeEnvelopeFromStream(string contentType, Stream serializedEnvelope, string messageType);
    }
}