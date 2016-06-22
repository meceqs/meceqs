using System;

namespace Meceqs
{
    public interface IEnvelopeFactory
    {
        Envelope<TMessage> Create<TMessage>(TMessage message, Guid messageId, MessageHeaders headers = null)
            where TMessage : IMessage;
    }
}