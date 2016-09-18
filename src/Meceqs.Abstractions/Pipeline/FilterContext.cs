using System;
using System.Security.Claims;
using System.Threading;

namespace Meceqs.Pipeline
{
    public class FilterContext<TMessage> : FilterContext where TMessage : class
    {
        public new Envelope<TMessage> Envelope => (Envelope<TMessage>)base.Envelope;

        public new TMessage Message => (TMessage)base.Message;

        public FilterContext(Envelope<TMessage> envelope)
            : base(envelope)
        {
        }
    }

    public abstract class FilterContext
    {
        public Envelope Envelope { get; }

        public object Message => Envelope.Message; // just for faster access to the message

        public Type MessageType => Envelope.Message.GetType();

        public Type ExpectedResultType { get; set; }

        public object Result { get; set; }

        public CancellationToken Cancellation { get; set; }

        public FilterContextItems Items { get; } = new FilterContextItems();

        public string PipelineName { get; set; }

        public IServiceProvider RequestServices { get; set; }

        public ClaimsPrincipal User { get; set; }

        protected FilterContext(Envelope envelope)
        {
            Check.NotNull(envelope, nameof(envelope));

            Envelope = envelope;
        }
    }
}