using System.Threading;

namespace Meceqs
{
    public interface IMessageContextFactory
    {
        MessageContext Create(Envelope envelope, MessageContextData contextData, CancellationToken cancellation);
    }
}