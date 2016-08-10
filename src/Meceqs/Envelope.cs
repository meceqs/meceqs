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
    }

    public abstract class Envelope
    {
        public IMessage Message { get; set; }
        public Guid MessageId { get; set; }
        public string MessageName { get; set; }
        public string MessageType { get; set; }
        public MessageHeaders Headers { get; set; } = new MessageHeaders();
        public Guid CorrelationId { get; set; } // TODO !! rename (ConversationId, TraceIdentifier, ...)
        public List<MessageHistoryEntry> MessageHistory { get; set; } = new List<MessageHistoryEntry>();

        public void SetHeader(string headerName, object value)
        {
            Headers.SetValue(headerName, value);
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