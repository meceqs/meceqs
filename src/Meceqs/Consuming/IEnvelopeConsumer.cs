using System.Collections.Generic;

namespace Meceqs.Consuming
{
    public interface IEnvelopeConsumer
    {
        IConsumeBuilder ForEnvelope(Envelope envelope);

        IConsumeBuilder ForEnvelopes(IList<Envelope> envelopes);
    }
}