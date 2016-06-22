using System.Threading;

namespace Meceqs.Handling
{
    public class DefaultHandleContextFactory : IHandleContextFactory
    {
        public HandleContext<TMessage> Create<TMessage>(MessageEnvelope<TMessage> envelope, CancellationToken cancellation)
            where TMessage : IMessage
        {
            return new HandleContext<TMessage>(envelope, cancellation);
        }
    }
}