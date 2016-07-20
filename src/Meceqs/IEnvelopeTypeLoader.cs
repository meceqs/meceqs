using System;

namespace Meceqs
{
    public interface IEnvelopeTypeLoader
    {
        Type LoadEnvelopeType(string messageType);
    }
}