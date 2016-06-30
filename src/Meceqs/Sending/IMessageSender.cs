using System;

namespace Meceqs.Sending
{
    public interface IMessageSender
    {
        ISendBuilder<TMessage> ForMessage<TMessage>(TMessage message, Guid messageId)
            where TMessage : IMessage;
    }

    public static class MessageSenderExtensions
    {
        public static ISendBuilder<TCommand> ForCommand<TCommand>(this IMessageSender sender, TCommand cmd, Guid messageId)
            where TCommand : ICommand
        {
            Check.NotNull(sender, nameof(sender));

            return sender.ForMessage<TCommand>(cmd, messageId);
        }

        public static ISendBuilder<TEvent> ForEvent<TEvent>(this IMessageSender sender, TEvent @event, Guid messageId, Envelope sourceMessage)
            where TEvent : IEvent
        {
            Check.NotNull(sender, nameof(sender));
            Check.NotNull(sourceMessage, nameof(sourceMessage));

            return sender.ForMessage(@event, messageId)
                .CorrelateWith(sourceMessage);
        }

        public static ISendBuilder<TQuery> ForQuery<TQuery>(this IMessageSender sender, TQuery query, Guid? messageId = null)
            where TQuery : IQuery
        {
            Check.NotNull(sender, nameof(sender));

            Guid id = messageId ?? Guid.NewGuid();

            return sender.ForMessage(query, id);
        }
    }
}