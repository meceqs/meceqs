using System.Threading;
using System.Threading.Tasks;
using Meceqs.Internal;

namespace Meceqs.Sending
{
    public class DefaultSendBuilder<TMessage> : ISendBuilder<TMessage>
        where TMessage : IMessage
    {
        private readonly IMessageCorrelator _messageCorrelator;
        private readonly IMessageSendingMediator _sendingMediator;

        private readonly Envelope<TMessage> _envelope;
        private readonly ContextData _sendContextData = new ContextData();

        private CancellationToken _cancellation = CancellationToken.None;

        public DefaultSendBuilder(
            Envelope<TMessage> envelope,
            IMessageCorrelator messageCorrelator,
            IMessageSendingMediator sendingMediator)
        {
            Check.NotNull(envelope, nameof(envelope));
            Check.NotNull(messageCorrelator, nameof(messageCorrelator));
            Check.NotNull(sendingMediator, nameof(sendingMediator));

            _envelope = envelope;
            _messageCorrelator = messageCorrelator;
            _sendingMediator = sendingMediator;
        }

        public ISendBuilder<TMessage> CorrelateWith(Envelope source)
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

        public ISendBuilder<TMessage> SetContextItem(string key, object value)
        {
            _sendContextData.Set(key, value);
            return this;
        }

        public SendContext<TMessage> BuildSendContext()
        {
            return new SendContext<TMessage>(_envelope, _sendContextData, _cancellation);
        }

        public Task SendAsync()
        {
            return SendAsync<VoidType>();
        }

        public Task<TResult> SendAsync<TResult>()
        {
            var sendContext = BuildSendContext();
            return _sendingMediator.SendAsync<TMessage, TResult>(sendContext);
        }
    }
}