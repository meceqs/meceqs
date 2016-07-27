using System;
using Microsoft.Extensions.Options;

namespace Meceqs.Sending
{
    public class DefaultEnvelopeFactory : IEnvelopeFactory
    {
        private readonly MeceqsOptions _options;

        public DefaultEnvelopeFactory(IOptions<MeceqsOptions> options)
        {
            Check.NotNull(options, nameof(options));

            _options = options.Value;
        }

        public Envelope Create(IMessage message, Guid messageId, MessageHeaders headers = null)
        {
            Check.NotNull(message, nameof(message));
            Check.NotEmpty(messageId, nameof(messageId));

            // TODO @cweiss Caching! https://rogeralsing.com/2008/02/28/linq-expressions-creating-objects/
            Type messageType = message.GetType();
            Type envelopeType = typeof(Envelope<>).MakeGenericType(messageType);
            var envelope = (Envelope)Activator.CreateInstance(envelopeType);

            SetProperties(envelope, message, messageId, headers);

            return envelope;
        }

        private void SetProperties(Envelope envelope, IMessage message, Guid messageId, MessageHeaders headers)
        {
            Type messageType = message.GetType();

            envelope.Headers = headers ?? new MessageHeaders();
            envelope.Message = message;
            envelope.MessageId = messageId;

            envelope.MessageName = messageType.Name;
            envelope.MessageType = messageType.FullName;

            // should be overwritten, if message is correlated with other message
            envelope.CorrelationId = Guid.NewGuid();

            envelope.SetHeader(MessageHeaderNames.CreatedOnUtc, DateTime.UtcNow); // TODO @cweiss IClock?

            envelope.SetHeader(MessageHeaderNames.SourceApplication, _options.ApplicationName);
            envelope.SetHeader(MessageHeaderNames.SourceHost, _options.HostName);
        }
    }
}