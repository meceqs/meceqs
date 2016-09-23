using Meceqs.Sending;

namespace Meceqs.TypedHandling
{
    /// <summary>
    /// Extension methods for <see cref="ISendBuilder"/>.
    /// </summary>
    public static class SendBuilderExtensions
    {
        /// <summary>
        /// States the the message to be sent "follows from" a previous message.
        /// This will correlate the messages and reuse properties from the orginal filter context
        /// (e.g. service provider, user, cancellation token).
        /// </summary>
        public static ISendBuilder FollowsFrom(this ISendBuilder builder, HandleContext context)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(context, nameof(context));

            builder.CorrelateWith(context.Envelope);

            builder.SetCancellationToken(context.FilterContext.Cancellation);
            builder.SetUser(context.FilterContext.User);

            return builder.Instance;
        }
    }
}