using System;
using System.IO;

namespace Meceqs.Serialization
{
    public interface IEnvelopeSerializer
    {
        string ContentType { get; }

        byte[] Serialize(Envelope envelope);

        Envelope Deserialize(Stream serializedEnvelope, Type envelopeType);
    }
}