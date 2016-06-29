using System;

namespace Meceqs
{
    public interface IEnvelopeSerializer
    {
        byte[] Serialize(Envelope envelope);

        Envelope Deserialize(byte[] serializedObject, Type messageType);
    }
}