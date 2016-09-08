using System;
using System.Collections.Generic;
using Meceqs.AspNetCore.Consuming;

namespace Microsoft.AspNetCore.Builder
{
    public class AspNetCoreConsumerOptions
    {
        public List<MessageMetadata> MessageTypes { get; } = new List<MessageMetadata>();

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