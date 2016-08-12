using System;
using System.Collections.Generic;
using Meceqs.Pipeline;

namespace Meceqs.Sending.Internal
{
    public class MessageSender : IMessageSender
    {
        private readonly IEnvelopeFactory _envelopeFactory;
        private readonly IEnvelopeCorrelator _envelopeCorrelator;
        private readonly IFilterContextFactory _filterContextFactory;
        private readonly IPipeline _pipeline;

        public MessageSender(
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

        public IFluentSender ForMessage<TMessage>(TMessage message) where TMessage : class
        {
            return ForMessage(message, Guid.NewGuid());
        }

        public IFluentSender ForMessage<TMessage>(TMessage message, Guid messageId) where TMessage : class
        {
            Check.NotNull(message, nameof(message));
            Check.NotEmpty(messageId, nameof(messageId));

            var envelope = _envelopeFactory.Create(message, messageId);

            var envelopes = new List<Envelope> { envelope };

            return new FluentSender(envelopes, _envelopeCorrelator, _filterContextFactory, _pipeline);
        }

        public IFluentSender ForMessages<TMessage>(IList<TMessage> messages) where TMessage : class
        {
            Check.NotNull(messages, nameof(messages));

            var envelopes = new List<Envelope>();

            foreach (var message in messages)
            {
                var messageId = Guid.NewGuid();
                var envelope = _envelopeFactory.Create(message, messageId);
            }

            return new FluentSender(envelopes, _envelopeCorrelator, _filterContextFactory, _pipeline);
        }
    }
}