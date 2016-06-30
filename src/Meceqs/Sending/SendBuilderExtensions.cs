namespace Meceqs.Sending
{
    public static class SendBuilderExtensions
    {
        public static ISendBuilder<TMessage> SetCreationReason<TMessage>(this ISendBuilder<TMessage> sender, string reason)
            where TMessage : IMessage
        {
            Check.NotNull(sender, nameof(sender));

            return sender.SetHeader(MessageHeaderNames.CreationReason, reason);
        }

        public static ISendBuilder<TMessage> UsePartitionKey<TMessage>(this ISendBuilder<TMessage> sender, object partitionKey)
            where TMessage : IMessage
        {
            Check.NotNull(sender, nameof(sender));

            return sender.SetContextItem("PartitionKey", partitionKey);
        }
    }
}