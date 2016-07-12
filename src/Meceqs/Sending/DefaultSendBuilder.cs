using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Sending
{
    public class DefaultSendBuilder<TMessage> : ISendBuilder<TMessage>
        where TMessage : IMessage
    {
        private readonly IMessageCorrelator _messageCorrelator;
        private readonly IMessageSendingMediator _sendingMediator;

        private readonly Envelope<TMessage> _envelope;
        private readonly MessageContextData _contextData = new MessageContextData();

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
            _contextData.Set(key, value);
            return this;
        }

        public MessageContext<TMessage> BuildContext()
        {
            return new MessageContext<TMessage>(_envelope, _contextData, _cancellation);
        }

        public Task SendAsync()
        {
            return SendAsync<VoidType>();
        }

        public Task<TResult> SendAsync<TResult>()
        {
            var context = BuildContext();
            return _sendingMediator.SendAsync<TMessage, TResult>(context);
        }
    }
}