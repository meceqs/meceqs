using System;
using Meceqs.Sending;
using Meceqs.Sending.Internal;

namespace Meceqs.Tests
{
    public class TestObjects
    {
        public static IEnvelopeFactory EnvelopeFactory()
        {
            return new DefaultEnvelopeFactory();
        }

        public static Envelope<TMessage> Envelope<TMessage>(TMessage message = null, Guid? id = null)
            where TMessage : class, new()
        {
            message = message ?? new TMessage();

            return (Envelope<TMessage>)EnvelopeFactory().Create(message, id ?? Guid.NewGuid());
        }
    }
}