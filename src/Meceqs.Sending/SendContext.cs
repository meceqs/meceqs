using System;
using System.Threading;

namespace Meceqs.Sending
{
    public class SendContext<TMessage> where TMessage : IMessage
    {
        private readonly SendProperties _sendProperties;

        public CancellationToken Cancellation { get; }

        public MessageEnvelope<TMessage> Envelope { get; }

        public SendContext(MessageEnvelope<TMessage> envelope, SendProperties sendProperties, CancellationToken? cancellation)
        {
            if (envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            _sendProperties = sendProperties ?? new SendProperties();
            Envelope = envelope;
            Cancellation = cancellation ?? CancellationToken.None;
        }

        public T GetSendProperty<T>(string key)
        {
            object value;
            if (key != null && _sendProperties.TryGetValue(key, out value))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }

            return default(T);
        }
    }
}