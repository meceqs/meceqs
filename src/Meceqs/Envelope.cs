using System;
using System.Collections.Generic;

namespace Meceqs
{
    public class Envelope<TMessage> : Envelope where TMessage : IMessage
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
        public IMessage Message { get; set; }
        public Guid MessageId { get; set; }
        public string MessageType { get; set; }
        public string MessageName { get; set; }
        public Guid? CorrelationId { get; set; } // TODO !! rename (ConversationId, TraceIdentifier, ...)
        public DateTime? CreatedOnUtc { get; set; }
        public MessageHeaders Headers { get; set; } = new MessageHeaders();
        public List<MessageHistoryEntry> History { get; set; } = new List<MessageHistoryEntry>();

        protected Envelope() 
        {
        }

        protected Envelope(IMessage message, Guid messageId)
        {
            Check.NotNull(message, nameof(message));
            Check.NotEmpty(messageId, nameof(messageId));

            Type messageType = message.GetType();

            Message = message;
            MessageName = messageType.Name;
            MessageType = messageType.FullName;

            MessageId = messageId;

            // should be overwritten, if message is correlated with other message
            CorrelationId = Guid.NewGuid();

            CreatedOnUtc = DateTime.UtcNow;
        }

        public void EnsureValid()
        {
            if (Headers == null)
                Headers = new MessageHeaders();

            Check.NotNull(Message, nameof(Message));
            Check.NotEmpty(MessageId, nameof(MessageId));
            Check.NotNullOrWhiteSpace(MessageName, nameof(MessageName));
            Check.NotNullOrWhiteSpace(MessageType, nameof(MessageType));
        }
    }
}