using System;

namespace Meceqs.Sending
{
    public interface IMessageClient
    {
        IMessageEnvelopeSender<TMessage> ForMessage<TMessage>(Guid messageId, TMessage message)
            where TMessage : IMessage;
    }

    public static class MessageClientExtensions
    {
        public static IMessageEnvelopeSender<TCommand> ForCommand<TCommand>(this IMessageClient client, Guid messageId, TCommand cmd)
            where TCommand : ICommand
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            return client.ForMessage<TCommand>(messageId, cmd);
        }

        public static IMessageEnvelopeSender<TEvent> ForEvent<TEvent>(this IMessageClient client, Guid messageId, TEvent evt, MessageEnvelope sourceMessage)
            where TEvent : IEvent
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            if (sourceMessage == null)
                throw new ArgumentNullException(nameof(sourceMessage));

            return client.ForMessage(messageId, evt)
                .CorrelateWith(sourceMessage);
        }

        public static IMessageEnvelopeSender<TQuery> ForQuery<TQuery>(this IMessageClient client, TQuery query)
            where TQuery : IQuery
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            Guid messageId = Guid.NewGuid();

            return client.ForMessage(messageId, query);
        }
    }
}