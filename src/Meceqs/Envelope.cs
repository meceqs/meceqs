using System;

namespace Meceqs
{
    public abstract class Envelope
    {
        public MessageHeaders Headers { get; set; } = new MessageHeaders();

        public Guid MessageId { get; set; }

        public IMessage Message { get; set; }

        public string MessageName { get; set; }

        public string MessageType { get; set; }

        public Guid CorrelationId { get; set; }

        public void SetHeader(string headerName, object value)
        {
            Headers.SetValue(headerName, value);
        }

        public void EnsureValid()
        {
            if (Headers == null)
                Headers = new MessageHeaders();

            if (Message == null)
                throw new ArgumentNullException(nameof(Message));

            if (MessageId == Guid.Empty)
                throw new ArgumentNullException(nameof(MessageId));

            if (string.IsNullOrWhiteSpace(MessageName))
                throw new ArgumentNullException(nameof(MessageName));

            if (string.IsNullOrEmpty(MessageType))
                throw new ArgumentNullException(nameof(MessageType));

            if (CorrelationId == Guid.Empty)
                throw new ArgumentNullException(nameof(CorrelationId));
        }
    }

    public class Envelope<TMessage> : Envelope where TMessage : IMessage
    {
        public new TMessage Message
        {
            get { return (TMessage)base.Message; }
            set { base.Message = value; }
        }
    }
}