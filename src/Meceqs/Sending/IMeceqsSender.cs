using System;
using System.Collections.Generic;

namespace Meceqs.Sending
{
    public interface IMeceqsSender
    {
        ISendBuilder ForMessage(IMessage message);

        ISendBuilder ForMessage(IMessage message, Guid messageId);

        ISendBuilder ForMessages(IList<IMessage> messages);
    }

    public static class MeceqsSenderExtensions
    {
        public static ISendBuilder ForCommand(this IMeceqsSender sender, ICommand cmd, Guid messageId)
        {
            Check.NotNull(sender, nameof(sender));

            return sender.ForMessage(cmd, messageId);
        }

        public static ISendBuilder ForEvent(this IMeceqsSender sender, IEvent @event, Guid messageId, Envelope sourceMessage)
        {
            Check.NotNull(sender, nameof(sender));
            Check.NotNull(sourceMessage, nameof(sourceMessage));

            return sender.ForMessage(@event, messageId)
                .CorrelateWith(sourceMessage);
        }

        public static ISendBuilder ForQuery(this IMeceqsSender sender, IQuery query, Guid? messageId = null)
        {
            Check.NotNull(sender, nameof(sender));

            Guid id = messageId ?? Guid.NewGuid();

            return sender.ForMessage(query, id);
        }
    }
}