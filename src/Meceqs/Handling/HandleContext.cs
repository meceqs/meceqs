using System.Threading;
using Meceqs.Internal;

namespace Meceqs.Handling
{
    public class HandleContext<TMessage> : ContextBase<TMessage>
        where TMessage : IMessage
    {
        public HandleContext(Envelope<TMessage> envelope, CancellationToken cancellation)
            : base(envelope, null, cancellation)
        {
        }
    }
}