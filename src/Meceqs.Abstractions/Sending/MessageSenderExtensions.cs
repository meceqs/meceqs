using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meceqs.Sending
{
    /// <summary>
    /// Extension methods for <see cref="IMessageSender"/>.
    /// </summary>
    public static class MessageSenderExtensions
    {
        /// <summary>
        /// Sends the message to the default "Send" pipeline. If you want to use a different pipeline
        /// or change some other behavior, use the builder pattern with <see cref="ForMessage"/>.
        /// </summary>
        public static Task SendAsync(this IMessageSender sender, object message, Guid? messageId = null)
        {
            Check.NotNull(sender, nameof(sender));

            return sender.ForMessage(message, messageId).SendAsync();
        }

        /// <summary>
        /// Sends the message to the default "Send" pipeline and expects a result object of the given type.
        /// If you want to use a different pipeline or change some other behavior,
        /// use the builder pattern with <see cref="ForMessage"/>.
        /// </summary>
        public static Task<TResult> SendAsync<TResult>(this IMessageSender sender, object message, Guid? messageId = null)
        {
            Check.NotNull(sender, nameof(sender));

            return sender.ForMessage(message, messageId).SendAsync<TResult>();
        }

        /// <summary>
        /// Sends the messages to the default "Send" pipeline. If you want to use a different pipeline
        /// or change some other behavior, use the builder pattern with <see cref="ForMessage"/>.
        /// </summary>
        public static Task SendAsync<TMessage>(this IMessageSender sender, IEnumerable<TMessage> messages)
            where TMessage : class
        {
            Check.NotNull(sender, nameof(sender));

            return sender.ForMessages<TMessage>(messages).SendAsync();
        }
    }
}