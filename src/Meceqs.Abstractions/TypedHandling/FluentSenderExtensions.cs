using Meceqs.Sending;

namespace Meceqs.TypedHandling
{
    public static class FluentSenderExtensions
    {
        /// <summary>
        /// States the the message to be sent "follows from" a previous message.
        /// This will correlate the messages and reuse properties from the orginal filter context
        /// (e.g. service provider, user, cancellation token).
        /// </summary>
        public static IFluentSender FollowsFrom(this IFluentSender sender, HandleContext context)
        {
            Check.NotNull(sender, nameof(sender));
            Check.NotNull(context, nameof(context));

            sender.CorrelateWith(context.Envelope);

            sender.SetCancellationToken(context.FilterContext.Cancellation);
            sender.SetRequestServices(context.FilterContext.RequestServices);
            sender.SetUser(context.FilterContext.User);

            return sender.Instance;
        }
    }
}