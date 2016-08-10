using System;
using System.Collections.Generic;
using System.Linq;

namespace Meceqs.Sending
{
    public static class MessageSenderExtensions
    {
        public static ISendBuilder ForCommand(this IMessageSender sender, ICommand cmd, Guid messageId)
        {
            Check.NotNull(sender, nameof(sender));

            return sender.ForMessage(cmd, messageId);
        }

        public static ISendBuilder ForEvent(this IMessageSender sender, IEvent @event, Guid messageId, Envelope sourceMessage)
        {
            Check.NotNull(sender, nameof(sender));
            Check.NotNull(sourceMessage, nameof(sourceMessage));

            return sender.ForMessage(@event, messageId)
                .CorrelateWith(sourceMessage);
        }

        public static ISendBuilder ForEvents(this IMessageSender sender, IList<IEvent> events, Envelope sourceMessage)
        {
            Check.NotNull(sender, nameof(sender));

            var messages = events.Cast<IMessage>().ToList();

            return sender.ForMessages(messages).CorrelateWith(sourceMessage);
        }

        public static ISendBuilder ForQuery(this IMessageSender sender, IQuery query, Guid? messageId = null)
        {
            Check.NotNull(sender, nameof(sender));

            Guid id = messageId ?? Guid.NewGuid();

            return sender.ForMessage(query, id);
        }
    }
}