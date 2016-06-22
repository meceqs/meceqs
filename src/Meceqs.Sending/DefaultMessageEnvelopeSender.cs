using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Sending
{
    public class DefaultMessageEnvelopeSender<TMessage> : IMessageEnvelopeSender<TMessage>
        where TMessage : IMessage
    {
        private readonly ISendTransport _sendTransport;
        private readonly IMessageCorrelator _messageCorrelator;

        private readonly MessageEnvelope<TMessage> _envelope;

        private readonly SendProperties _sendProperties = new SendProperties();

        private CancellationToken? _cancellation;

        public DefaultMessageEnvelopeSender(
            ISendTransport sendTransport,
            IMessageCorrelator messageCorrelator,
            MessageEnvelope<TMessage> envelope)
        {
            if (sendTransport == null)
                throw new ArgumentNullException(nameof(sendTransport));

            if (messageCorrelator == null)
                throw new ArgumentNullException(nameof(messageCorrelator));

            if (envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            _sendTransport = sendTransport;
            _messageCorrelator = messageCorrelator;

            _envelope = envelope;
        }

        public IMessageEnvelopeSender<TMessage> CorrelateWith(MessageEnvelope source)
        {
            _messageCorrelator.CorrelateSourceWithTarget(source, _envelope);
            return this;
        }

        public IMessageEnvelopeSender<TMessage> SetCancellationToken(CancellationToken cancellation)
        {
            _cancellation = cancellation;
            return this;
        }

        public IMessageEnvelopeSender<TMessage> SetHeader(string headerName, object value)
        {
            _envelope.SetHeader(headerName, value);
            return this;
        }

        public IMessageEnvelopeSender<TMessage> SetSendProperty(string key, object value)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                _sendProperties[key] = value;
            }

            return this;
        }
        
        public async Task SendAsync()
        {
            await SendAsync<VoidType>();
        }

        public async Task<TResult> SendAsync<TResult>()
        {
            var sendContext = BuildSendContext();
            return await _sendTransport.SendAsync<TMessage, TResult>(sendContext);
        }

        private SendContext<TMessage> BuildSendContext()
        {
            return new SendContext<TMessage>(_envelope, _sendProperties, _cancellation);
        }
    }
}