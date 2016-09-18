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

        public static FilterContext<TMessage> FilterContext<TMessage>(Type resultType = null, IServiceProvider requestServices = null)
            where TMessage : class, new()
        {
            var envelope = TestObjects.Envelope<TMessage>();

            var filterContext = new FilterContext<TMessage>(envelope);
            filterContext.RequestServices = requestServices ?? Substitute.For<IServiceProvider>();
            filterContext.ExpectedResultType = resultType;

            return filterContext;
        }

        public static HandleContext<TMessage> HandleContext<TMessage>(Type resultType, FilterContext<TMessage> filterContext = null)
            where TMessage : class, new()
        {
            filterContext = filterContext ?? FilterContext<TMessage>(resultType);

            return new HandleContext<TMessage>(filterContext);
        }
    }
}