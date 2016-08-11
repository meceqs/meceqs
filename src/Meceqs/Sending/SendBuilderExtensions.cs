namespace Meceqs.Sending
{
    public static class SendBuilderExtensions
    {
        public static IFluentSender SetCreationReason(this IFluentSender sendBuilder, string reason)
        {
            Check.NotNull(sendBuilder, nameof(sendBuilder));

            return sendBuilder.SetHeader(MessageHeaderNames.CreationReason, reason);
        }

        public static IFluentSender UsePartitionKey(this IFluentSender sendBuilder, object partitionKey)
        {
            Check.NotNull(sendBuilder, nameof(sendBuilder));

            return sendBuilder.SetContextItem("PartitionKey", partitionKey);
        }
    }
}