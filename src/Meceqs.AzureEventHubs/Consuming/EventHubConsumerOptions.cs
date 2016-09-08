using System;
using System.Collections.Generic;
using Meceqs.Configuration;

namespace Meceqs.AzureEventHubs.Consuming
{
    public class EventHubConsumerOptions
    {
        public List<Type> MessageTypes { get; set; } = new List<Type>();

        public UnknownMessageBehavior UnknownMessageBehavior { get; set; } = UnknownMessageBehavior.ThrowException;

        public void AddMessageType<TMessage>()
        {
            AddMessageType(typeof(TMessage));
        }

        public void AddMessageType(Type messageType)
        {
            Check.NotNull(messageType, nameof(messageType));

            MessageTypes.Add(messageType);
        }
    }
}