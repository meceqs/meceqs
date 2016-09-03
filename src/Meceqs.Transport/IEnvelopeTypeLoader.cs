using System;

namespace Meceqs.Serialization
{
    public interface IEnvelopeTypeLoader
    {
        Type LoadEnvelopeType(string messageType);
    }
}