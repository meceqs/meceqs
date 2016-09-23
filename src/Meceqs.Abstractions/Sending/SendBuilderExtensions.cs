namespace Meceqs.Sending
{
    /// <summary>
    /// Extension methods for <see cref="ISendBuilder"/>.
    /// </summary>
    public static class SendBuilderExtensions
    {
        // TODO @cweiss !! where should we put these things? (and do we even want them?)

        public static ISendBuilder SetCreationReason(this ISendBuilder builder, string reason)
        {
            Check.NotNull(builder, nameof(builder));

            return builder.SetHeader("CreationReason", reason);
        }

        public static ISendBuilder UsePartitionKey(this ISendBuilder builder, object partitionKey)
        {
            Check.NotNull(builder, nameof(builder));

            return builder.SetContextItem("PartitionKey", partitionKey);
        }
    }
}