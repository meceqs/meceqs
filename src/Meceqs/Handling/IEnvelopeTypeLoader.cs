using System;

namespace Meceqs.Handling
{
    public interface IEnvelopeTypeLoader
    {
        Type LoadEnvelopeType(string messageType);
    }
}