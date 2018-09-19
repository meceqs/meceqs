using System;
using System.Threading.Tasks;

namespace Meceqs.Receiving
{
    public class MessageReceiver : IMessageReceiver
    {
        private readonly IServiceProvider _serviceProvider;

        public MessageReceiver(IServiceProvider serviceProvider)
        {
            Guard.NotNull(serviceProvider, nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public IReceiveBuilder ForEnvelope(Envelope envelope)
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

            return new ReceiveBuilder(envelope, _serviceProvider);
        }

        /// <summary>
        /// Sends the envelope to the default "Receive" pipeline. If you want to use a different pipeline
        /// or change some other behavior, use the builder pattern with <see cref="ForEnvelope"/>.
        /// </summary>
        public Task ReceiveAsync(Envelope envelope)
        {
            return ForEnvelope(envelope).ReceiveAsync();
        }

        /// <summary>
        /// Sends the envelope to the default "Receive" pipeline and expects a response object of the given type.
        /// If you want to use a different pipeline or change some other behavior,
        /// use the builder pattern with <see cref="ForEnvelope"/>.
        /// </summary>
        public Task<TResponse> ReceiveAsync<TResponse>(Envelope envelope)
        {
            return ForEnvelope(envelope).ReceiveAsync<TResponse>();
        }

        /// <summary>
        /// Sends the envelope to the default "Receive" pipeline and expects a response object of the given type.
        /// If you want to use a different pipeline or change some other behavior,
        /// use the builder pattern with <see cref="ForEnvelope"/>.
        /// </summary>
        public Task<object> ReceiveAsync(Envelope envelope, Type responseType)
        {
            return ForEnvelope(envelope).ReceiveAsync(responseType);
        }
    }
}
