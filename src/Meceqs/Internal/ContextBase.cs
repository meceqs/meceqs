using System.Threading;

namespace Meceqs.Internal
{
    public abstract class ContextBase<TMessage> where TMessage : IMessage
    {
        private readonly ContextData _data;

        public CancellationToken Cancellation { get; set; } // setter allows decorators to change the token (for whatever reason).

        public Envelope<TMessage> Envelope { get; }

        public TMessage Message => Envelope.Message; // just for easier access to the property

        protected ContextBase(Envelope<TMessage> envelope, ContextData contextData, CancellationToken cancellation)
        {
            Check.NotNull(envelope, nameof(envelope));

            Envelope = envelope;
            _data = contextData ?? new ContextData();
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
}