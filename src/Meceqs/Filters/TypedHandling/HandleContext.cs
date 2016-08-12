using System;
using System.Reflection;
using Meceqs.Pipeline;

namespace Meceqs.Filters.TypedHandling
{
    public class HandleContext<TMessage> : HandleContext where TMessage : class
    {
        public new Envelope<TMessage> Envelope => (Envelope<TMessage>)base.Envelope;

        public new TMessage Message => (TMessage)base.Message;

        public HandleContext(FilterContext<TMessage> filterContext)
            : base(filterContext.Envelope, filterContext.Message)
        {
        }
    }

    public abstract class HandleContext
    {
        public Envelope Envelope { get; }

        public object Message { get; }

        public IHandles Handler { get; set; }
        public Type HandlerType { get; set; }

        public MethodInfo HandleMethod { get; set; }

        protected HandleContext(Envelope envelope, object message)
        {
            Check.NotNull(envelope, nameof(envelope));
            Check.NotNull(message, nameof(message));

            Envelope = envelope;
            Message = message;
        }
    }
}