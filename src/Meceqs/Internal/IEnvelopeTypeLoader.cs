using System;

namespace Meceqs.Internal
{
    public interface IEnvelopeTypeLoader
    {
        /// <summary>
        /// Returns the non-generic envelope type for the given message type.
        /// </summary>
        Type LoadEnvelopeType(string messageType);
    }
}
