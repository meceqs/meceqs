using System;

namespace Meceqs.Sending
{
    public static class OpinionatedSendBuilderExtensions
    {
        // TODO @cweiss do we want something like this?

        public static ISendBuilder<TMessage> SetCreationReason<TMessage>(this ISendBuilder<TMessage> sender, string reason)
            where TMessage : IMessage
        {
            if (sender == null)
                throw new ArgumentNullException(nameof(sender));

            return sender.SetHeader("CreationReason", reason);
        }

        public static ISendBuilder<TMessage> SetSendPartitionKey<TMessage>(this ISendBuilder<TMessage> sender, object partitionKey)
            where TMessage : IMessage
        {
            if (sender == null)
                throw new ArgumentNullException(nameof(sender));

            return sender.SetContextItem("PartitionKey", partitionKey);
        }
    }
}