using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meceqs.Sending
{
    public static class MessageSenderExtensions
    {
        /// <summary>
        /// Shortcut for <code>ForMessage(message, messageId).SendAsync()</code>.
        /// </summary>
        public static Task SendAsync(this IMessageSender sender, object message, Guid? messageId = null)
        {
            Check.NotNull(sender, nameof(sender));

            return sender.ForMessage(message, messageId).SendAsync();
        }

        /// <summary>
        /// Shortcut for <code>ForMessage(message, messageId).SendAsync&lt;TResult&gt;()</code>.
        /// </summary>
        public static Task<TResult> SendAsync<TResult>(this IMessageSender sender, object message, Guid? messageId = null)
        {
            Check.NotNull(sender, nameof(sender));

            return sender.ForMessage(message, messageId).SendAsync<TResult>();
        }

        /// <summary>
        /// Shortcut for <code>ForMessages(messages).SendAsync()</code>.
        /// </summary>
        public static Task SendAsync(this IMessageSender sender, IList<object> messages)
        {
            Check.NotNull(sender, nameof(sender));

            return sender.ForMessages(messages).SendAsync();
        }

        /// <summary>
        /// Shortcut for <code>ForMessages(messages).SendAsync&lt;TResult&gt;()</code>.
        /// </summary>
        public static Task<TResult> SendAsync<TResult>(this IMessageSender sender, IList<object> messages)
        {
            Check.NotNull(sender, nameof(sender));

            return sender.ForMessages(messages).SendAsync<TResult>();
        }
    }
}