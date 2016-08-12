using System;

namespace Meceqs.Sending
{
    public interface IEnvelopeFactory
    {
        Envelope Create(object message, Guid messageId);
    }
}