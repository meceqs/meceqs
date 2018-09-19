using System;

namespace Meceqs.Sending
{
    public interface IEnvelopeFactory
    {
        /// <summary>
        /// Creates a typed <see cref="Envelope"/> for the given <paramref name="message"/>.
        /// </summary>
        Envelope Create(object message, Guid messageId);
    }
}
