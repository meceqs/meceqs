using Meceqs.Pipeline;

namespace Meceqs.Filters.TypedHandling
{
    public class HandleContext<TMessage> : HandleContext where TMessage : IMessage
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

        public IMessage Message { get; }

        protected HandleContext(Envelope envelope, IMessage message)
        {
            Check.NotNull(envelope, nameof(envelope));
            Check.NotNull(message, nameof(message));

            Envelope = envelope;
            Message = message;
        }
    }
}