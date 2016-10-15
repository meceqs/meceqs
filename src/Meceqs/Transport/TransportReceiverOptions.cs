using System;
using System.Collections.Generic;

namespace Meceqs.Transport
{
    public abstract class TransportReceiverOptions
    {
        public List<MessageMetadata> MessageTypes { get; } = new List<MessageMetadata>();

        public UnknownMessageBehavior UnknownMessageBehavior { get; set; } = UnknownMessageBehavior.ThrowException;

        public void AddMessageType<TMessage>()
        {
            AddMessageType(typeof(TMessage));
        }

        public void AddMessageType<TMessage, TResult>()
        {
            AddMessageType(typeof(TMessage), typeof(TResult));
        }

        public void AddMessageType(Type messageType, Type resultType = null)
        {
            MessageTypes.Add(new MessageMetadata(messageType, resultType));
        }
    }
}