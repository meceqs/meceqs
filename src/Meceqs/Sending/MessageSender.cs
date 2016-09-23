using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Sending
{
    public class MessageSender : IMessageSender
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEnvelopeFactory _envelopeFactory;

        public MessageSender(IServiceProvider serviceProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            _serviceProvider = serviceProvider;
            _envelopeFactory = serviceProvider.GetRequiredService<IEnvelopeFactory>();
        }

        public IFluentSender ForEnvelope(Envelope envelope)
        {
            Check.NotNull(envelope, nameof(envelope));

            // If we get an envelope from an external system, we want to make sure
            // that the minimum requirements are satisfied.

            // Especially in cases like HTTP where people can easily create their own
            // requests with e.g. Fiddler, it's possible that not all envelope properties are present.
            // For this reason, we try to add as much as possible.
            envelope.Sanitize();

            // However, if there's still a required property missing, we have to give up as soon as possible.
            envelope.EnsureValid();

            return new FluentSender(envelope, _serviceProvider);
        }

        public IFluentSender ForMessage(object message, Guid? messageId = null)
        {
            Check.NotNull(message, nameof(message));

            var envelope = _envelopeFactory.Create(message, messageId ?? Guid.NewGuid());

            return ForEnvelope(envelope);
        }

        public IFluentSender ForMessages<TMessage>(IEnumerable<TMessage> messages) where TMessage : class
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

            return new FluentSender(envelopes, _serviceProvider);
        }
    }
}