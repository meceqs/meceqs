using System.Threading;
using Meceqs.Internal;

namespace Meceqs.Sending
{
    public class SendContext<TMessage> : ContextBase<TMessage>
        where TMessage : IMessage
    {
        public SendContext(Envelope<TMessage> envelope, ContextData contextData, CancellationToken cancellation)
            : base(envelope, contextData, cancellation)
        {
        }
    }
}