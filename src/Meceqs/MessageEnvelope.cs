﻿using System;

namespace Meceqs
{
    public class MessageEnvelope<TMessage> : IMessageEnvelope where TMessage : IMessage
    {
        public MessageHeaders Headers { get; set; } = new MessageHeaders();

        public Guid MessageId { get; set; }

        public TMessage Message { get; set; }

        public string MessageName { get; set; }

        public string MessageType { get; set; }

        public Guid CorrelationId { get; set; }


        public MessageEnvelope(TMessage message, Guid messageId)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (messageId == Guid.Empty)
                throw new ArgumentNullException(nameof(messageId));

            Message = message;
            MessageId = messageId;

            Type messageType = message.GetType();
            MessageName = messageType.Name;
            MessageType = messageType.FullName;

            // will be overwritten, if message is correlated with other message
            CorrelationId = Guid.NewGuid();
        }

        public void SetHeader(string headerName, object value)
        {
            Headers.SetValue(headerName, value);
        }
    }
}