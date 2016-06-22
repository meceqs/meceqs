using System.Threading;

namespace Meceqs.Handling
{
    public interface IHandleContextFactory
    {
        HandleContext<TMessage> Create<TMessage>(MessageEnvelope<TMessage> envelope, CancellationToken cancellation)
            where TMessage : IMessage;
    }
}