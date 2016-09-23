using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public IFluentSender ForMessages(IEnumerable<object> messages)
        {
            Check.NotNull(messages, nameof(messages));

            Guid? correlationId = null;
            var envelopes = new List<Envelope>();

            foreach (var message in messages)
            {
                var envelope = _envelopeFactory.Create(message, messageId: Guid.NewGuid());

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

        /// <summary>
        /// Sends the message to the default "Send" pipeline. If you want to use a different pipeline
        /// or change some other behavior, use the builder pattern with <see cref="ForMessage"/>.
        /// </summary>
        public Task SendAsync(object message, Guid? messageId = null)
        {
            return ForMessage(message, messageId).SendAsync();
        }

        /// <summary>
        /// Sends the message to the default "Send" pipeline and expects a result object of the given type.
        /// If you want to use a different pipeline or change some other behavior,
        /// use the builder pattern with <see cref="ForMessage"/>.
        /// </summary>
        public Task<TResult> SendAsync<TResult>(object message, Guid? messageId = null)
        {
            return ForMessage(message, messageId).SendAsync<TResult>();
        }

        /// <summary>
        /// Sends the messages to the default "Send" pipeline. If you want to use a different pipeline
        /// or change some other behavior, use the builder pattern with <see cref="ForMessage"/>.
        /// </summary>
        public Task SendAsync(IEnumerable<object> messages)
        {
            return ForMessages(messages).SendAsync();
        }
    }
}