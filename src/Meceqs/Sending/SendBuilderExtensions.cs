namespace Meceqs.Sending
{
    public static class SendBuilderExtensions
    {
        public static ISendBuilder SetCreationReason(this ISendBuilder sendBuilder, string reason)
        {
            Check.NotNull(sendBuilder, nameof(sendBuilder));

            return sendBuilder.SetHeader(MessageHeaderNames.CreationReason, reason);
        }

        public static ISendBuilder UsePartitionKey(this ISendBuilder sendBuilder, object partitionKey)
        {
            Check.NotNull(sendBuilder, nameof(sendBuilder));

            return sendBuilder.SetContextItem("PartitionKey", partitionKey);
        }
    }
}