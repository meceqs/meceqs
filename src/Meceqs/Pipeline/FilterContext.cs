using System;
using System.Threading;

namespace Meceqs.Pipeline
{
    public class FilterContext<TMessage> : FilterContext where TMessage : IMessage
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
        private readonly FilterContextItems _items = new FilterContextItems();

        public Envelope Envelope { get; }

        public IMessage Message => Envelope.Message; // just for faster access to the message

        public Type ExpectedResultType { get; set; }

        public object Result { get; set; }

        public IServiceProvider RequestServices { get; set; }

        public CancellationToken Cancellation { get; set; }

        public string ChannelName { get; set; }

        protected FilterContext(Envelope envelope)
        {
            Check.NotNull(envelope, nameof(envelope));

            Envelope = envelope;
        }

        public T GetContextItem<T>(string key)
        {
            return _items.Get<T>(key);
        }

        public void SetContextItem(string key, object value)
        {
            _items.Set(key, value);
        }
    }
}