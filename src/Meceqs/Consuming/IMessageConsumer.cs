using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meceqs.Consuming
{
    public interface IMessageConsumer
    {
        IFluentConsumer ForEnvelope(Envelope envelope);

        IFluentConsumer ForEnvelopes(IList<Envelope> envelopes);

        Task ConsumeAsync(Envelope envelope);

        Task<TResult> ConsumeAsync<TResult>(Envelope envelope);

        Task ConsumeAsync(IList<Envelope> envelopes);

        Task<TResult> ConsumeAsync<TResult>(IList<Envelope> envelopes);
    }
}