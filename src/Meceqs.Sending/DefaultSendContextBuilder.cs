using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Sending
{
    public class DefaultSendContextBuilder : ISendContextBuilder
    {
        private readonly IMessageClient _messageClient;
        private readonly IMessageCorrelator _messageCorrelator;

        private readonly SendContext _sendContext;

        public DefaultSendContextBuilder(
            IMessageClient messageClient,
            IMessageCorrelator messageCorrelator,
            MessageEnvelope envelope,
            CancellationToken cancellation)
        {
            if (messageClient == null)
                throw new ArgumentNullException(nameof(messageClient));

            if (envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            _messageClient = messageClient;
            _messageCorrelator = messageCorrelator;

            _sendContext = new SendContext(envelope, cancellation);
        }

        public ISendContextBuilder CorrelateWith(MessageEnvelope source)
        {
            _messageCorrelator.CorrelateSourceWithTarget(source, _sendContext.Envelope);
            return this;
        }

        public ISendContextBuilder SetHeader(string headerName, object value)
        {
            _sendContext.Envelope.SetHeader(headerName, value);
            return this;
        }

        public ISendContextBuilder SetSendProperty(string key, object value)
        {
            _sendContext.SetSendProperty(key, value);
            return this;
        }

        public async Task<TResult> SendAsync<TResult>()
        {
            return await _messageClient.SendAsync<TResult>(_sendContext);
        }
    }
}