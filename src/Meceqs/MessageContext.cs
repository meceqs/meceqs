using System.Threading;

namespace Meceqs
{
    public abstract class MessageContext
    {
        private readonly MessageContextData _data;

        public CancellationToken Cancellation { get; set; } // setter allows decorators to change the token (for whatever reason).

        public Envelope Envelope { get; }

        public IMessage Message => Envelope.Message; // just for faster access to the message

        public MessageContext(Envelope envelope, MessageContextData contextData, CancellationToken cancellation)
        {
            Check.NotNull(envelope, nameof(envelope));

            Envelope = envelope;
            _data = contextData ?? new MessageContextData();
            Cancellation = cancellation;
        }

        public T GetContextItem<T>(string key)
        {
            return _data.Get<T>(key);
        }

        public void SetContextItem(string key, object value)
        {
            _data.Set(key, value);
        }
    }

    public class MessageContext<TMessage> : MessageContext where TMessage : IMessage
    {
        public new Envelope<TMessage> Envelope => (Envelope<TMessage>)base.Envelope;

        public new TMessage Message => (TMessage)base.Message;

        public MessageContext(Envelope<TMessage> envelope, MessageContextData contextData, CancellationToken cancellation)
            : base(envelope, contextData, cancellation)
        {
        }
    }
}