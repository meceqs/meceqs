using System.Threading.Tasks;

namespace Meceqs.Consuming
{
    /// <summary>
    /// Extension methods for <see cref="IMessageConsumer"/>.
    /// </summary>
    public static class MessageConsumerExtensions
    {
        /// <summary>
        /// Sends the envelope to the default "Consume" pipeline. If you want to use a different pipeline
        /// or change some other behavior, use the builder pattern with <see cref="ForEnvelope"/>.
        /// </summary>
        public static Task ConsumeAsync(this IMessageConsumer consumer, Envelope envelope)
        {
            Check.NotNull(consumer, nameof(consumer));

            return consumer.ForEnvelope(envelope).ConsumeAsync();
        }

        /// <summary>
        /// Sends the envelope to the default "Consume" pipeline and expects a result object of the given type.
        /// If you want to use a different pipeline or change some other behavior,
        /// use the builder pattern with <see cref="ForEnvelope"/>.
        /// </summary>
        public static Task<TResult> ConsumeAsync<TResult>(this IMessageConsumer consumer, Envelope envelope)
        {
            Check.NotNull(consumer, nameof(consumer));

            return consumer.ForEnvelope(envelope).ConsumeAsync<TResult>();
        }
    }
}