using System;

namespace Meceqs
{
    public interface IEnvelopeFactory
    {
        Envelope Create(IMessage message, Guid messageId);
    }
}