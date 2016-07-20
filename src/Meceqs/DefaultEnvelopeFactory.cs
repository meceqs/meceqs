using System;
using Microsoft.Extensions.Options;

namespace Meceqs
{
    public class DefaultEnvelopeFactory : IEnvelopeFactory
    {
        private readonly MeceqsOptions _options;

        public DefaultEnvelopeFactory(IOptions<MeceqsOptions> options)
        {
            Check.NotNull(options, nameof(options));

            _options = options.Value;
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

            envelope.SetHeader(MessageHeaderNames.CreatedOnUtc, DateTime.UtcNow); // TODO @cweiss IClock?

            envelope.SetHeader(MessageHeaderNames.SourceApplication, _options.ApplicationName);
            envelope.SetHeader(MessageHeaderNames.SourceHost, _options.HostName);

            return envelope;
        }
    }
}