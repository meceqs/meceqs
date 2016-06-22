using System;
using System.Security.Claims;
using System.Threading;

namespace Meceqs.Handling
{
    public class HandleContext<TMessage> where TMessage : IMessage
    {
        public CancellationToken Cancellation { get; set; }

        public MessageHeaders Headers { get; set; }

        public TMessage Message { get; set; }

        public Guid MessageId { get; set; }

        public ClaimsPrincipal User { get; set; } // TODO @cweiss move this to sample with derived class?

        public HandleContext()
        {
        }

        public HandleContext(MessageEnvelope<TMessage> envelope, CancellationToken cancellation)
        {
            if (envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            Headers = envelope.Headers;
            Message = envelope.Message;
            MessageId = envelope.MessageId;

            Cancellation = cancellation;
        }
    }
}