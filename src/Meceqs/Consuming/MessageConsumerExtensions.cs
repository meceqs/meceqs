using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meceqs.Consuming
{
    public static class MessageConsumerExtensions
    {
        /// <summary>
        /// Shortcut for <code>ForEnvelope(envelope).ConsumeAsync()</code>.
        /// </summary>
        public static Task ConsumeAsync(this IMessageConsumer consumer, Envelope envelope)
        {
            Check.NotNull(consumer, nameof(consumer));

            return consumer.ForEnvelope(envelope).ConsumeAsync();
        }

        /// <summary>
        /// Shortcut for <code>ForEnvelope(envelope).ConsumeAsync&lt;TResult&gt;()</code>.
        /// </summary>
        public static Task<TResult> ConsumeAsync<TResult>(this IMessageConsumer consumer, Envelope envelope)
        {
            Check.NotNull(consumer, nameof(consumer));

            return consumer.ForEnvelope(envelope).ConsumeAsync<TResult>();
        }

        /// <summary>
        /// Shortcut for <code>ForEnvelopes(envelopes).ConsumeAsync()</code>.
        /// </summary>
        public static Task ConsumeAsync(this IMessageConsumer consumer, IList<Envelope> envelopes)
        {
            Check.NotNull(consumer, nameof(consumer));

            return consumer.ForEnvelopes(envelopes).ConsumeAsync();
        }

        /// <summary>
        /// Shortcut for <code>ForEnvelopes(envelopes).ConsumeAsync&lt;TResult&gt;()</code>.
        /// </summary>
        public static Task<TResult> ConsumeAsync<TResult>(this IMessageConsumer consumer, IList<Envelope> envelopes)
        {
            Check.NotNull(consumer, nameof(consumer));

            return consumer.ForEnvelopes(envelopes).ConsumeAsync<TResult>();
        }
    }
}