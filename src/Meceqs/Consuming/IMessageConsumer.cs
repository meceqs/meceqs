using System.Collections.Generic;

namespace Meceqs.Consuming
{
    public interface IMessageConsumer
    {
        IFluentConsumer ForEnvelope(Envelope envelope);

        IFluentConsumer ForEnvelopes(IList<Envelope> envelopes);
    }
}