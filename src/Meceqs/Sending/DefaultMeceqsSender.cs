using System;
using System.Collections.Generic;

namespace Meceqs.Sending
{
    public class DefaultMeceqsSender : IMeceqsSender
    {
        private readonly IEnvelopeFactory _envelopeFactory;
        private readonly IEnvelopeCorrelator _envelopeCorrelator;
        private readonly IMessageContextFactory _messageContextFactory;
        private readonly IMessageSendingMediator _sendingMediator;

        public DefaultMeceqsSender(
            IEnvelopeFactory envelopeFactory,
            IEnvelopeCorrelator envelopeCorrelator,
            IMessageContextFactory messageContextFactory,
            IMessageSendingMediator sendingMediator)
        {
            Check.NotNull(envelopeFactory, nameof(envelopeFactory));
            Check.NotNull(envelopeCorrelator, nameof(envelopeCorrelator));
            Check.NotNull(messageContextFactory, nameof(messageContextFactory));
            Check.NotNull(sendingMediator, nameof(sendingMediator));

            _envelopeFactory = envelopeFactory;
            _envelopeCorrelator = envelopeCorrelator;
            _messageContextFactory = messageContextFactory;
            _sendingMediator = sendingMediator;
        }

        public ISendBuilder ForMessage(IMessage message)
        {
            return ForMessage(message, Guid.NewGuid());
        }

        public ISendBuilder ForMessage(IMessage message, Guid messageId)
        {
            Check.NotNull(message, nameof(message));
            Check.NotEmpty(messageId, nameof(messageId));

            var envelope = _envelopeFactory.Create(message, messageId);

            var envelopes = new List<Envelope> { envelope };

            return new DefaultSendBuilder(envelopes, _envelopeCorrelator, _messageContextFactory, _sendingMediator);
        }

        public ISendBuilder ForMessages(IList<IMessage> messages)
        {
            Check.NotNull(messages, nameof(messages));

            var envelopes = new List<Envelope>();

            foreach (var message in messages)
            {
                var messageId = Guid.NewGuid();
                var envelope = _envelopeFactory.Create(message, messageId);
            }

            return new DefaultSendBuilder(envelopes, _envelopeCorrelator, _messageContextFactory, _sendingMediator);
        }
    }
}