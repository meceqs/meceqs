using System;
using System.Collections.Generic;

namespace Meceqs.Transport
{
    public abstract class ReceiveTransportOptions
    {
        public List<MessageMetadata> MessageTypes { get; } = new List<MessageMetadata>();

        public UnknownMessageBehavior UnknownMessageBehavior { get; set; } = UnknownMessageBehavior.ThrowException;

        public void AddMessageType(Type messageType, Type resultType = null)
        {
            MessageTypes.Add(new MessageMetadata(messageType, resultType));
        }
    }
}