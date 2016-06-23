using System;
using Microsoft.Extensions.Options;

namespace Meceqs
{
    public class DefaultEnvelopeFactory : IEnvelopeFactory
    {
        private readonly ApplicationInfo _applicationInfo;

        public DefaultEnvelopeFactory(IOptions<ApplicationInfo> applicationInfo)
        {
            if (applicationInfo == null)
                throw new ArgumentNullException(nameof(applicationInfo));

            _applicationInfo = applicationInfo.Value;
        }

        public Envelope<TMessage> Create<TMessage>(TMessage message, Guid messageId, MessageHeaders headers = null)
            where TMessage : IMessage
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

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

            envelope.Headers.SetValue(MessageHeaderNames.CreatedOnUtc, DateTime.UtcNow);
            envelope.Headers.SetValue(MessageHeaderNames.SourceApplication, _applicationInfo.ApplicationName);
            envelope.Headers.SetValue(MessageHeaderNames.SourceHost, _applicationInfo.HostName);

            return envelope;
        }
    }
}