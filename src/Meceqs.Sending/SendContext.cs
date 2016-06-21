using System;
using System.Threading;

namespace Meceqs.Sending
{
    public class SendContext<TMessage> where TMessage : IMessage
    {
        public CancellationToken Cancellation { get; set; }

        public MessageEnvelope<TMessage> Envelope { get; set; }

        public SendProperties SendProperties { get; set; }

        public SendContext()
        {
        }

        public SendContext(MessageEnvelope<TMessage> envelope, SendProperties sendProperties, CancellationToken cancellation)
        {
            if (envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            Envelope = envelope;
            SendProperties = sendProperties ?? new SendProperties();
            Cancellation = cancellation;
        }
    }
}