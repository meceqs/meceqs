using System;

namespace Meceqs.Tests
{
    public class TestObjects
    {
        public static IEnvelopeFactory EnvelopeFactory()
        {
            return new DefaultEnvelopeFactory();
        }

        public static Envelope<TMessage> Envelope<TMessage>(TMessage message = null, Guid? id = null)
            where TMessage : class, IMessage, new()
        {
            message = message ?? new TMessage();

            return (Envelope<TMessage>)EnvelopeFactory().Create(message, id ?? Guid.NewGuid());
        }
    }
}