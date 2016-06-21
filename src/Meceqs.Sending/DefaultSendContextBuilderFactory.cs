using System.Threading;

namespace Meceqs.Sending
{
    public class DefaultSendContextBuilderFactory : ISendContextBuilderFactory
    {
        private readonly IMessageCorrelator _messageCorrelator;

        public DefaultSendContextBuilderFactory(IMessageCorrelator messageCorrelator)
        {
            _messageCorrelator = messageCorrelator;
        }

        public ISendContextBuilder Create(IMessageClient messageClient, MessageEnvelope envelope, CancellationToken cancellation)
        {
            return new DefaultSendContextBuilder(messageClient, _messageCorrelator, envelope, cancellation);
        }
    }
}