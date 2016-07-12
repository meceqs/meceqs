using System;

namespace Meceqs
{
    public class DefaultEnvelopeFactory : IEnvelopeFactory
    {
        private readonly ApplicationInfo _applicationInfo;

        public DefaultEnvelopeFactory(ApplicationInfo applicationInfo)
        {
            Check.NotNull(applicationInfo, nameof(applicationInfo));

            _applicationInfo = applicationInfo;
        }

        public Envelope<TMessage> Create<TMessage>(TMessage message, Guid messageId, MessageHeaders headers = null)
            where TMessage : IMessage
        {
            Check.NotNull(message, nameof(message));
            Check.NotEmpty(messageId, nameof(messageId));

            Type messageType = message.GetType();

            var envelope = new Envelope<TMessage>
            {
                Headers = headers ?? new MessageHeaders(),

                Message = message,
                MessageId = messageId,

                MessageName = messageType.Name,
                MessageType = messageType.FullName,

                // should be overwritten, if message is correlated with other message
                CorrelationId = Guid.NewGuid()
            };

            envelope.SetHeader(MessageHeaderNames.CreatedOnUtc, DateTime.UtcNow);
            envelope.SetHeader(MessageHeaderNames.SourceApplication, _applicationInfo.ApplicationName);
            envelope.SetHeader(MessageHeaderNames.SourceHost, _applicationInfo.HostName);

            return envelope;
        }
    }
}