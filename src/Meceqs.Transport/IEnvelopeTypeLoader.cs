using System;

namespace Meceqs.Transport
{
    public interface IEnvelopeTypeLoader
    {
        Type LoadEnvelopeType(string messageType);
    }
}