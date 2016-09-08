using Meceqs.Sending;

namespace Meceqs.TypedHandling
{
    public static class FluentSenderExtensions
    {
        /// <summary>
        /// States the the message to be sent "follows from" a previous message.
        /// This will correlate the messages and reuse the service provider.
        /// </summary>
        public static IFluentSender FollowsFrom(this IFluentSender sender, HandleContext context)
        {
            Check.NotNull(sender, nameof(sender));
            Check.NotNull(context, nameof(context));

            sender.CorrelateWith(context.Envelope);

            sender.SetRequestServices(context.FilterContext.RequestServices);

            return sender.Instance;
        }
    }
}