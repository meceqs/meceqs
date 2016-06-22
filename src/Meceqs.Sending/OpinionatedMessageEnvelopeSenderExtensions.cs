using System;

namespace Meceqs.Sending
{
    public static class OpinionatedMessageEnvelopeSenderExtensions
    {
        // TODO @cweiss do we want something like this?

        public static IMessageEnvelopeSender<TMessage> SetCreationReason<TMessage>(this IMessageEnvelopeSender<TMessage> sender, string reason)
            where TMessage : IMessage
        {
            if (sender == null)
                throw new ArgumentNullException(nameof(sender));

            return sender.SetHeader("CreationReason", reason);
        }

        public static IMessageEnvelopeSender<TMessage> SetSendPartitionKey<TMessage>(this IMessageEnvelopeSender<TMessage> sender, object partitionKey)
            where TMessage : IMessage
        {
            if (sender == null)
                throw new ArgumentNullException(nameof(sender));

            return sender.SetSendProperty("PartitionKey", partitionKey);
        }
    }
}