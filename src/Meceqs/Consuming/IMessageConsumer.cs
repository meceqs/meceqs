using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meceqs.Consuming
{
    public interface IMessageConsumer
    {
        IFluentConsumer ForEnvelope(Envelope envelope);

        IFluentConsumer ForEnvelopes(IList<Envelope> envelopes);

        /// <summary>
        /// Shortcut for <code>ForEnvelope(envelope).ConsumeAsync()</code>.
        /// </summary>
        Task ConsumeAsync(Envelope envelope);

        /// <summary>
        /// Shortcut for <code>ForEnvelope(envelope).ConsumeAsync&lt;TResult&gt;()</code>.
        /// </summary>
        Task<TResult> ConsumeAsync<TResult>(Envelope envelope);

        /// <summary>
        /// Shortcut for <code>ForEnvelopes(envelopes).ConsumeAsync()</code>.
        /// </summary>
        Task ConsumeAsync(IList<Envelope> envelopes);

        /// <summary>
        /// Shortcut for <code>ForEnvelopes(envelopes).ConsumeAsync&lt;TResult&gt;()</code>.
        /// </summary>
        Task<TResult> ConsumeAsync<TResult>(IList<Envelope> envelopes);
    }
}