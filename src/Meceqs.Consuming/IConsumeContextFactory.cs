using System.Threading;

namespace Meceqs.Consuming
{
    public interface IConsumeContextFactory
    {
        ConsumeContext<TMessage> Create<TMessage>(MessageEnvelope<TMessage> envelope, CancellationToken cancellation) where TMessage : IMessage;
    }
}