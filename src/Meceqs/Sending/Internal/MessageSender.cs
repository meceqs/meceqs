using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Meceqs.Sending.Internal
{
    public class MessageSender : IMessageSender
    {
        private readonly IEnvelopeFactory _envelopeFactory;
        private readonly IEnvelopeCorrelator _envelopeCorrelator;
        private readonly IFilterContextFactory _filterContextFactory;
        private readonly IPipelineProvider _pipelineProvider;

        public MessageSender(
            IEnvelopeFactory envelopeFactory,
            IEnvelopeCorrelator envelopeCorrelator,
            IFilterContextFactory filterContextFactory,
            IPipelineProvider pipelineProvider)
        {
            Check.NotNull(envelopeFactory, nameof(envelopeFactory));
            Check.NotNull(envelopeCorrelator, nameof(envelopeCorrelator));
            Check.NotNull(filterContextFactory, nameof(filterContextFactory));
            Check.NotNull(pipelineProvider, nameof(pipelineProvider));

            _envelopeFactory = envelopeFactory;
            _envelopeCorrelator = envelopeCorrelator;
            _filterContextFactory = filterContextFactory;
            _pipelineProvider = pipelineProvider;
        }

        public IFluentSender ForMessage(object message)
        {
            return ForMessage(message, Guid.NewGuid());
        }

        public IFluentSender ForMessage(object message, Guid messageId)
        {
            Check.NotNull(message, nameof(message));
            Check.NotEmpty(messageId, nameof(messageId));

            var envelope = _envelopeFactory.Create(message, messageId);

            var envelopes = new List<Envelope> { envelope };

            return new FluentSender(envelopes, _envelopeCorrelator, _filterContextFactory, _pipelineProvider);
        }

        public IFluentSender ForMessages<TMessage>(IList<TMessage> messages) where TMessage : class
        {
            Check.NotNull(messages, nameof(messages));

            Guid? correlationId = null;
            var envelopes = new List<Envelope>();

            foreach (var message in messages)
            {
                var messageId = Guid.NewGuid();

                var envelope = _envelopeFactory.Create(message, messageId);

                // In case no-one correlates these messages with another message, we at least want
                // all of them to have the same correlation-id (from the first message).
                if (correlationId.HasValue)
                {
                    envelope.CorrelationId = correlationId.Value;
                }
                else
                {
                    correlationId = envelope.CorrelationId;
                }
                
                envelopes.Add(envelope);
            }

            return new FluentSender(envelopes, _envelopeCorrelator, _filterContextFactory, _pipelineProvider);
        }

        public Task SendAsync(object message, Guid? messageId = null)
        {
            return ForMessage(message, messageId ?? Guid.NewGuid())
                .SendAsync();
        }

        public Task<TResult> SendAsync<TResult>(object message, Guid? messageId = null)
        {
            return ForMessage(message, messageId ?? Guid.NewGuid())
                .SendAsync<TResult>();
        }
    }
}