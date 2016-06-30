using System;
using System.IO;

namespace Meceqs
{
    public interface IEnvelopeSerializer
    {
        string ContentType { get; }

        byte[] Serialize(Envelope envelope);

        Envelope Deserialize(Stream serializedEnvelope, Type envelopeType);
    }
}