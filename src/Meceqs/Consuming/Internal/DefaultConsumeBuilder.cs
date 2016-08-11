using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Meceqs.Consuming.Internal
{
    public class DefaultConsumeBuilder : IConsumeBuilder
    {
        private const string PipelineName = "Consume";

        private readonly IFilterContextFactory _filterContextFactory;
        private readonly IPipeline _pipeline;
        private readonly IList<Envelope> _envelopes;
        private readonly FilterContextItems _contextItems = new FilterContextItems();

        private CancellationToken _cancellation = CancellationToken.None;

        public DefaultConsumeBuilder(
            IList<Envelope> envelopes,
            IFilterContextFactory filterContextFactory,
            IPipeline pipeline)
        {
            Check.NotNull(envelopes, nameof(envelopes));
            Check.NotNull(filterContextFactory, nameof(filterContextFactory));
            Check.NotNull(pipeline, nameof(pipeline));

            _envelopes = envelopes;
            _filterContextFactory = filterContextFactory;
            _pipeline = pipeline;
        }

        public IConsumeBuilder SetCancellationToken(CancellationToken cancellation)
        {
            _cancellation = cancellation;
            return this;
        }

        public IConsumeBuilder SetContextItem(string key, object value)
        {
            _contextItems.Set(key, value);
            return this;
        }

        public Task ConsumeAsync()
        {
            return ConsumeAsync<VoidType>();
        }

        public Task<TResult> ConsumeAsync<TResult>()
        {
            var filterContexts = _envelopes.Select(CreateFilterContext).ToList();

            return _pipeline.SendAsync<TResult>(filterContexts);
        }

        private FilterContext CreateFilterContext(Envelope envelope)
        {
            var context = _filterContextFactory.CreateFilterContext(envelope);

            context.Cancellation = _cancellation;
            context.PipelineName = PipelineName;

            if (_contextItems.Count > 0)
            {
                foreach (var kvp in _contextItems)
                {
                    context.SetContextItem(kvp.Key, kvp.Value);
                }
            }

            return context;
        }
    }
}