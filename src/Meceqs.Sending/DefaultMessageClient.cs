using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Sending
{
    public class DefaultMessageClient : IMessageClient
    {
        private readonly ISendContextBuilderFactory _sendContextBuilderFactory;

        public DefaultMessageClient(ISendContextBuilderFactory sendContextBuilderFactory)
        {
            _sendContextBuilderFactory = sendContextBuilderFactory;
        }

        public ISendContextBuilder CreateMessage(Guid messageId, IMessage message, CancellationToken cancellation)
        {
            var envelope = new MessageEnvelope(messageId, message);
            return _sendContextBuilderFactory.Create(this, envelope, cancellation);
        }

        public Task<TResult> SendAsync<TResult>(SendContext context)
        {
            throw new NotImplementedException();
        }
    }
}