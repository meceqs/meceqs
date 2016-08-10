using System.Collections.Generic;
using Meceqs.Pipeline;

namespace Meceqs.Consuming.Internal
{
    public class DefaultEnvelopeConsumer : IEnvelopeConsumer
    {
        private readonly IFilterContextFactory _filterContextFactory;
        private readonly IConsumeChannel _consumeChannel;

        public DefaultEnvelopeConsumer(
            IFilterContextFactory filterContextFactory,
            IConsumeChannel consumeChannel)
        {
            Check.NotNull(filterContextFactory, nameof(filterContextFactory));
            Check.NotNull(consumeChannel, nameof(consumeChannel));

            _filterContextFactory = filterContextFactory;
            _consumeChannel = consumeChannel;
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

            return new DefaultConsumeBuilder(envelopes, _filterContextFactory, _consumeChannel.Channel);
        }
    }
}