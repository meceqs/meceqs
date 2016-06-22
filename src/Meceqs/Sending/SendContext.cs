using System;
using System.Threading;
using Meceqs.Internal;

namespace Meceqs.Sending
{
    public class SendContext<TMessage> : IHasContextData
        where TMessage : IMessage
    {
        private readonly ContextData _properties;

        public CancellationToken Cancellation { get; }

        public MessageEnvelope<TMessage> Envelope { get; }

        public SendContext(MessageEnvelope<TMessage> envelope, ContextData sendProperties, CancellationToken? cancellation)
        {
            if (envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            Envelope = envelope;
            _properties = sendProperties ?? new ContextData();
            Cancellation = cancellation ?? CancellationToken.None;
        }

        public T GetContextItem<T>(string key)
        {
            return _properties.Get<T>(key);
        }

        public void SetContextItem(string key, object value)
        {
            _properties.Set(key, value);
        }
    }
}