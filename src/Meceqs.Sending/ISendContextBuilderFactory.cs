using System.Threading;

namespace Meceqs.Sending
{
    public interface ISendContextBuilderFactory
    {
        ISendContextBuilder Create(IMessageClient messageClient, MessageEnvelope envelope, CancellationToken cancellation);
    }
}