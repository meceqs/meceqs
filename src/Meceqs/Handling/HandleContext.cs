using System;
using System.Threading;
using Meceqs.Internal;

namespace Meceqs.Handling
{
    public class HandleContext<TMessage> : IHasContextData
        where TMessage : IMessage
    {
        private readonly ContextData _data = new ContextData();

        public CancellationToken Cancellation { get; }

        public MessageHeaders Headers { get; }

        public TMessage Message { get; }

        public Guid MessageId { get; }

        public HandleContext(MessageEnvelope<TMessage> envelope, CancellationToken cancellation)
        {
            if (envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            if (envelope.Message == null)
                throw new ArgumentNullException($"{nameof(envelope)}.{nameof(envelope.Message)}");

            Headers = envelope.Headers ?? new MessageHeaders();
            Message = envelope.Message;
            MessageId = envelope.MessageId;

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