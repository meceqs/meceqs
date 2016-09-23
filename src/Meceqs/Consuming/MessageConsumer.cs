using System;
using System.Threading.Tasks;

namespace Meceqs.Consuming
{
    public class MessageConsumer : IMessageConsumer
    {
        private readonly IServiceProvider _serviceProvider;

        public MessageConsumer(IServiceProvider serviceProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public IFluentConsumer ForEnvelope(Envelope envelope)
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

            return new FluentConsumer(envelope, _serviceProvider);
        }

        /// <summary>
        /// Sends the envelope to the default "Consume" pipeline. If you want to use a different pipeline
        /// or change some other behavior, use the builder pattern with <see cref="ForEnvelope"/>.
        /// </summary>
        public Task ConsumeAsync(Envelope envelope)
        {
            return ForEnvelope(envelope).ConsumeAsync();
        }

        /// <summary>
        /// Sends the envelope to the default "Consume" pipeline and expects a result object of the given type.
        /// If you want to use a different pipeline or change some other behavior,
        /// use the builder pattern with <see cref="ForEnvelope"/>.
        /// </summary>
        public Task<TResult> ConsumeAsync<TResult>(Envelope envelope)
        {
            return ForEnvelope(envelope).ConsumeAsync<TResult>();
        }
    }
}