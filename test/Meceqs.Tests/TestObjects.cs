using System;
using Meceqs.Pipeline;
using Meceqs.Sending;
using Meceqs.TypedHandling;
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

        public static MessageContext<TMessage> MessageContext<TMessage>(Type resultType = null, IServiceProvider requestServices = null)
            where TMessage : class, new()
        {
            var envelope = TestObjects.Envelope<TMessage>();
            requestServices = requestServices ?? Substitute.For<IServiceProvider>();

            var messageContext = new MessageContext<TMessage>(envelope);
            messageContext.Initialize("pipeline", requestServices, resultType);

            return messageContext;
        }

        public static HandleContext<TMessage> HandleContext<TMessage>(Type resultType, MessageContext<TMessage> messageContext = null)
            where TMessage : class, new()
        {
            messageContext = messageContext ?? MessageContext<TMessage>(resultType);

            return new HandleContext<TMessage>(messageContext);
        }
    }
}