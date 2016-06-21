using System;

namespace Meceqs
{
    public class MessageEnvelope<TMessage> where TMessage : IMessage
    {
        public MessageHeaders Headers { get; set; } = new MessageHeaders();

        public Guid MessageId { get; set; }

        public TMessage Message { get; set; }

        public string MessageName { get; set; }

        public string MessageType { get; set; }

        public MessageEnvelope(Guid messageId, TMessage message)
        {
            if (messageId == Guid.Empty)
                throw new ArgumentNullException(nameof(messageId));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            MessageId = messageId;
            Message = message;

            Type messageType = message.GetType();
            MessageName = messageType.Name;
            MessageType = messageType.FullName;
        }
    }
}