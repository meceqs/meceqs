using System;
using System.Collections.Generic;

namespace Meceqs
{
    public class Envelope<TMessage> : Envelope where TMessage : class
    {
        public new TMessage Message
        {
            get { return (TMessage)base.Message; }
            set { base.Message = value; }
        }

        public Envelope()
        {
        }

        public Envelope(TMessage message, Guid messageId)
            : base(message, messageId)
        {
        }
    }

    public abstract class Envelope
    {
        public object Message { get; set; }
        public Guid MessageId { get; set; }
        public string MessageType { get; set; }
        public string MessageName { get; set; }
        public Guid? CorrelationId { get; set; }
        public DateTime? CreatedOnUtc { get; set; }
        public EnvelopeProperties Headers { get; set; } = new EnvelopeProperties();
        public List<EnvelopeHistoryEntry> History { get; set; } = new List<EnvelopeHistoryEntry>();

        protected Envelope()
        {
        }

        protected Envelope(object message, Guid messageId)
        {
            Check.NotNull(message, nameof(message));
            Check.NotEmpty(messageId, nameof(messageId));

            Message = message;
            MessageId = messageId;
            
            Sanitize();
        }

        public void EnsureValid()
        {
            if (Headers == null)
                Headers = new EnvelopeProperties();

            Check.NotNull(Message, nameof(Message));
            Check.NotEmpty(MessageId, nameof(MessageId));
            Check.NotNullOrWhiteSpace(MessageName, nameof(MessageName));
            Check.NotNullOrWhiteSpace(MessageType, nameof(MessageType));
        }

        public void Sanitize()
        {
            if (Message != null)
            {
                Type messageType = Message.GetType();

                // if the envelope is not deserialized in a statically typed way (e.g. through ASP.NET MVC ModelBinding),
                // the values for MessageName and -Type could be wrong or missing. (e.g. because the request was made with Fiddler)
                // For this reason, we just re-set them!
                MessageName = messageType.Name;
                MessageType = messageType.FullName;
            }

            if (MessageId == Guid.Empty)
            {
                MessageId = Guid.NewGuid();
            }

            if (!CorrelationId.HasValue || CorrelationId.Value == Guid.Empty)
            {
                // should be overwritten, if message is correlated with other message
                CorrelationId = Guid.NewGuid();
            }

            if (!CreatedOnUtc.HasValue)
            {
                CreatedOnUtc = DateTime.UtcNow;
            }
        }
    }
}