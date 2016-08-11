using System.Collections.Generic;
using Meceqs.Pipeline;

namespace Meceqs.Consuming.Internal
{
    public class DefaultEnvelopeConsumer : IEnvelopeConsumer
    {
        private readonly IFilterContextFactory _filterContextFactory;
        private readonly IPipeline _pipeline;

        public DefaultEnvelopeConsumer(
            IFilterContextFactory filterContextFactory,
            IConsumePipeline consumePipeline)
        {
            Check.NotNull(filterContextFactory, nameof(filterContextFactory));
            Check.NotNull(consumePipeline, nameof(consumePipeline));

            _filterContextFactory = filterContextFactory;
            _pipeline = consumePipeline.Pipeline;
        }

        public IConsumeBuilder ForEnvelope(Envelope envelope)
        {
            Check.NotNull(envelope, nameof(envelope));

            var envelopes = new List<Envelope> { envelope };

            return ForEnvelopes(envelopes);
        }

        public IConsumeBuilder ForEnvelopes(IList<Envelope> envelopes)
        {
            Check.NotNull(envelopes, nameof(envelopes));

            return new DefaultConsumeBuilder(envelopes, _filterContextFactory, _pipeline);
        }
    }
}