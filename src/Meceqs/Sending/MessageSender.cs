using System;
using System.Threading.Tasks;
using Meceqs.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Sending
{
    public class MessageSender : IMessageSender
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEnvelopeFactory _envelopeFactory;

        public MessageSender(IServiceProvider serviceProvider)
        {
            Guard.NotNull(serviceProvider, nameof(serviceProvider));

            _serviceProvider = serviceProvider;
            _envelopeFactory = serviceProvider.GetRequiredService<IEnvelopeFactory>();
        }

        /// <summary>
        /// Uses the builder pattern for sending an envelope to a pipeline.
        /// </summary>
        public ISendBuilder ForEnvelope(Envelope envelope)
        {
            Guard.NotNull(envelope, nameof(envelope));

            // If we get an envelope from an external system, we want to make sure
            // that the minimum requirements are satisfied.

            // Especially in cases like HTTP where people can easily create their own
            // requests with e.g. Fiddler, it's possible that not all envelope properties are present.
            // For this reason, we try to add as much as possible.
            envelope.Sanitize();

            // However, if there's still a required property missing, we have to give up as soon as possible.
            envelope.EnsureValid();

            return new SendBuilder(envelope, _serviceProvider);
        }

        /// <summary>
        /// Uses the builder pattern for sending a message to a pipeline.
        /// </summary>
        public ISendBuilder ForMessage(object message, Guid? messageId = null)
        {
            Guard.NotNull(message, nameof(message));

            var envelope = _envelopeFactory.Create(message, messageId ?? Guid.NewGuid());

            return ForEnvelope(envelope);
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
        /// Sends the message to the default "Send" pipeline and expects a response object of the given type.
        /// If you want to use a different pipeline or change some other behavior,
        /// use the builder pattern with <see cref="ForMessage"/>.
        /// </summary>
        public Task<TResponse> SendAsync<TResponse>(object message, Guid? messageId = null)
        {
            return ForMessage(message, messageId).SendAsync<TResponse>();
        }
    }
}
