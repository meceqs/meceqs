using System;
using Meceqs.Pipeline;
using Meceqs.Sending;
using NSubstitute;

namespace Meceqs.Tests
{
    public static class TestObjects
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

        public static MessageContext MessageContext<TMessage>(Type responseType = null, IServiceProvider requestServices = null)
            where TMessage : class, new()
        {
            var envelope = Envelope<TMessage>();
            requestServices = requestServices ?? Substitute.For<IServiceProvider>();

            return new MessageContext(envelope, "pipeline", requestServices, responseType ?? typeof(void));
        }
    }
}