using Meceqs.Sending;

namespace Meceqs.TypedHandling
{
    /// <summary>
    /// Extension methods for <see cref="ISendBuilder"/>.
    /// </summary>
    public static class SendBuilderExtensions
    {
        /// <summary>
        /// States that the message to be sent is a "child of" a parent message.
        /// This will correlate the messages and reuse properties from the orginal message context
        /// (e.g. service provider, user, cancellation token).
        /// </summary>
        public static ISendBuilder ChildOf(this ISendBuilder builder, HandleContext context)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNull(context, nameof(context));

            builder.CorrelateWith(context.Envelope);

            builder.SetCancellationToken(context.Cancellation);
            builder.SetUser(context.User);

            return builder.Instance;
        }

        /// <summary>
        /// States that the message to be sent "follows from" a previous message.
        /// This will correlate the messages and reuse properties from the orginal message context
        /// (e.g. service provider, user, cancellation token).
        /// </summary>
        public static ISendBuilder FollowsFrom(this ISendBuilder builder, HandleContext context)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNull(context, nameof(context));

            builder.CorrelateWith(context.Envelope);

            builder.SetCancellationToken(context.Cancellation);
            builder.SetUser(context.User);

            return builder.Instance;
        }
    }
}
