using System;

namespace Meceqs
{
    public class DefaultEnvelopeFactory : IEnvelopeFactory
    {
        public Envelope<TMessage> Create<TMessage>(TMessage message, Guid messageId, MessageHeaders headers = null)
            where TMessage : IMessage
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            Type messageType = message.GetType();

            return new Envelope<TMessage>
            {
                Headers = headers ?? new MessageHeaders(),

                Message = message,
                MessageId = messageId,

                MessageName = messageType.Name,
                MessageType = messageType.FullName,

                // should be overwritten, if message is correlated with other message
                CorrelationId = Guid.NewGuid()
            };
        }
    }
}