using System;

namespace Meceqs
{
    public interface IEnvelopeTypeConverter
    {
        Type ConvertToEnvelopeType(string messageType);
    }
}