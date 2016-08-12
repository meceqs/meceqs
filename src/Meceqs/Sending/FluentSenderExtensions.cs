namespace Meceqs.Sending
{
    public static class FluentSenderExtensions
    {
        public static IFluentSender SetCreationReason(this IFluentSender sender, string reason)
        {
            Check.NotNull(sender, nameof(sender));

            return sender.SetHeader("CreationReason", reason);
        }

        public static IFluentSender UsePartitionKey(this IFluentSender sender, object partitionKey)
        {
            Check.NotNull(sender, nameof(sender));

            return sender.SetContextItem("PartitionKey", partitionKey);
        }
    }
}