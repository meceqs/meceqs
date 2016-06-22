using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Sending
{
    public class DefaultSendBuilder<TMessage> : ISendBuilder<TMessage>
        where TMessage : IMessage
    {
        private readonly IMessageCorrelator _messageCorrelator;

        private readonly MessageEnvelope<TMessage> _envelope;

        private readonly SendProperties _sendProperties = new SendProperties();

        private ISendTransport _sendTransport;
        private CancellationToken? _cancellation;

        public DefaultSendBuilder(MessageEnvelope<TMessage> envelope, IMessageCorrelator messageCorrelator)
        {
            if (envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            if (messageCorrelator == null)
                throw new ArgumentNullException(nameof(messageCorrelator));

            _envelope = envelope;
            _messageCorrelator = messageCorrelator;
        }

        public ISendBuilder<TMessage> UseTransport(ISendTransport sendTransport)
        {
            if (sendTransport == null)
                throw new ArgumentNullException(nameof(sendTransport));

            _sendTransport = sendTransport;
            return this;
        }

        public ISendBuilder<TMessage> CorrelateWith(IMessageEnvelope source)
        {
            _messageCorrelator.CorrelateSourceWithTarget(source, _envelope);
            return this;
        }

        public ISendBuilder<TMessage> SetCancellationToken(CancellationToken cancellation)
        {
            _cancellation = cancellation;
            return this;
        }

        public ISendBuilder<TMessage> SetHeader(string headerName, object value)
        {
            _envelope.SetHeader(headerName, value);
            return this;
        }

        public ISendBuilder<TMessage> SetSendProperty(string key, object value)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                _sendProperties[key] = value;
            }

            return this;
        }

        public SendContext<TMessage> BuildSendContext()
        {
            return new SendContext<TMessage>(_envelope, _sendProperties, _cancellation);
        }

        public async Task SendAsync()
        {
            await SendAsync<VoidType>();
        }

        public async Task<TResult> SendAsync<TResult>()
        {
            if (_sendTransport == null)
                throw new InvalidOperationException($"{nameof(UseTransport)} was not called.");

            var sendContext = BuildSendContext();
            return await _sendTransport.SendAsync<TMessage, TResult>(sendContext);
        }
    }
}