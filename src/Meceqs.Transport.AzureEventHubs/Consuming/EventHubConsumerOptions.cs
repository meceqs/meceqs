using System;
using System.Collections.Generic;

namespace Meceqs.Transport.AzureEventHubs.Consuming
{
    public class EventHubConsumerOptions
    {
        public List<string> MessageTypes { get; set; } = new List<string>();

        public UnknownMessageBehavior UnknownMessageBehavior { get; set; } = UnknownMessageBehavior.ThrowException;

        public void AddMessageType<TMessage>()
        {
            AddMessageType(typeof(TMessage));
        }

        public void AddMessageType(Type messageType)
        {
            Check.NotNull(messageType, nameof(messageType));

            MessageTypes.Add(messageType.FullName);
        }

        public void AddMessageType(string messageType)
        {
            Check.NotNullOrWhiteSpace(messageType, nameof(messageType));

            MessageTypes.Add(messageType);
        }
    }
}