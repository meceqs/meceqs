using System.Threading;

namespace Meceqs.Consuming
{
    public class DefaultConsumeContextFactory : IConsumeContextFactory
    {
        public ConsumeContext<TMessage> Create<TMessage>(MessageEnvelope<TMessage> envelope, CancellationToken cancellation)
            where TMessage : IMessage
        {
            return new ConsumeContext<TMessage>(envelope, cancellation);
        }
    }
}