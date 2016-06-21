using System;

namespace Meceqs.Sending
{
    public static class SendContextBuilderExtensions
    {
        // TODO @cweiss do we want something like this?

        public static ISendContextBuilder SetCreationReason(this ISendContextBuilder builder, string reason)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.SetHeader("CreationReason", reason);
        }

        public static ISendContextBuilder SetSendPartitionKey(this ISendContextBuilder builder, object partitionKey)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.SetSendProperty("PartitionKey", partitionKey);
        }
    }
}