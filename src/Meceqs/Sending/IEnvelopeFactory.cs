using System;

namespace Meceqs.Sending
{
    public interface IEnvelopeFactory
    {
        Envelope Create(IMessage message, Guid messageId);
    }
}