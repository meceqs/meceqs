using System.IO;

namespace Meceqs.Transport
{
    public interface IEnvelopeDeserializer
    {
        Envelope DeserializeFromStream(string contentType, Stream serializedEnvelope, string messageType);
    }
}