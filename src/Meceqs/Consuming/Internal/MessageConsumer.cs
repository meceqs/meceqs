using System.Collections.Generic;
using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Meceqs.Consuming.Internal
{
    public class MessageConsumer : IMessageConsumer
    {
        private readonly IFilterContextFactory _filterContextFactory;
        private readonly IPipelineProvider _pipelineProvider;

        public MessageConsumer(
            IFilterContextFactory filterContextFactory,
            IPipelineProvider pipelineProvider)
        {
            Check.NotNull(filterContextFactory, nameof(filterContextFactory));
            Check.NotNull(pipelineProvider, nameof(pipelineProvider));

            _filterContextFactory = filterContextFactory;
            _pipelineProvider = pipelineProvider;
        }

        public IFluentConsumer ForEnvelope(Envelope envelope)
        {
            Check.NotNull(envelope, nameof(envelope));

            var envelopes = new List<Envelope> { envelope };

            return ForEnvelopes(envelopes);
        }

        public IFluentConsumer ForEnvelopes(IList<Envelope> envelopes)
        {
            Check.NotNull(envelopes, nameof(envelopes));

            return new FluentConsumer(envelopes, _filterContextFactory, _pipelineProvider);
        }

        public Task ConsumeAsync(Envelope envelope)
        {
            return ForEnvelope(envelope).ConsumeAsync();
        }

        public Task<TResult> ConsumeAsync<TResult>(Envelope envelope)
        {
            return ForEnvelope(envelope).ConsumeAsync<TResult>();
        }

        public Task ConsumeAsync(IList<Envelope> envelopes)
        {
            return ForEnvelopes(envelopes).ConsumeAsync();
        }

        public Task<TResult> ConsumeAsync<TResult>(IList<Envelope> envelopes)
        {
            return ForEnvelopes(envelopes).ConsumeAsync<TResult>();
        }
    }
}