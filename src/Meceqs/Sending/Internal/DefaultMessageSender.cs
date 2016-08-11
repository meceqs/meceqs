using System;
using System.Collections.Generic;
using Meceqs.Pipeline;

namespace Meceqs.Sending.Internal
{
    public class DefaultMessageSender : IMessageSender
    {
        private readonly IEnvelopeFactory _envelopeFactory;
        private readonly IEnvelopeCorrelator _envelopeCorrelator;
        private readonly IFilterContextFactory _filterContextFactory;
        private readonly IPipeline _pipeline;

        public DefaultMessageSender(
            IEnvelopeFactory envelopeFactory,
            IEnvelopeCorrelator envelopeCorrelator,
            IFilterContextFactory filterContextFactory,
            ISendPipeline sendPipeline)
        {
            Check.NotNull(envelopeFactory, nameof(envelopeFactory));
            Check.NotNull(envelopeCorrelator, nameof(envelopeCorrelator));
            Check.NotNull(filterContextFactory, nameof(filterContextFactory));
            Check.NotNull(sendPipeline, nameof(sendPipeline));

            _envelopeFactory = envelopeFactory;
            _envelopeCorrelator = envelopeCorrelator;
            _filterContextFactory = filterContextFactory;
            _pipeline = sendPipeline.Pipeline;
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

            return new DefaultSendBuilder(envelopes, _envelopeCorrelator, _filterContextFactory, _pipeline);
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

            return new DefaultSendBuilder(envelopes, _envelopeCorrelator, _filterContextFactory, _pipeline);
        }
    }
}