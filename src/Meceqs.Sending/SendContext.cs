using System;
using System.Threading;

namespace Meceqs.Sending
{
    public class SendContext
    {
        public CancellationToken Cancellation { get; set; }

        public MessageEnvelope Envelope { get; set; }

        public SendProperties SendProperties { get; set; } = new SendProperties();

        public SendContext(MessageEnvelope envelope, CancellationToken cancellation)
        {
            if (envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            Envelope = envelope;
            Cancellation = cancellation;
        }

        public void SetSendProperty(string key, object value)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                SendProperties[key] = value;
            }
        }
    }
}