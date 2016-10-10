using System.IO;

namespace Meceqs.Serialization
{
    public interface IEnvelopeDeserializer
    {
        Envelope DeserializeEnvelope(string contentType, byte[] serializedEnvelope, string messageType);

        Envelope DeserializeEnvelope(string contentType, Stream serializedEnvelope, string messageType);
    }
}