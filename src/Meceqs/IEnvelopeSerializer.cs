using System;

namespace Meceqs
{
    public interface IEnvelopeSerializer
    {
        string Serialize(Envelope envelope);

        Envelope Deserialize(string serializedEnvelope, Type messageType);
    }
}