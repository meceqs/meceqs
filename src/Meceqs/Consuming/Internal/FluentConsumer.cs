using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Meceqs.Consuming.Internal
{
    public class FluentConsumer : IFluentConsumer
    {
        private readonly IList<Envelope> _envelopes;
        private readonly FilterContextItems _contextItems = new FilterContextItems();
        private readonly IFilterContextFactory _filterContextFactory;
        private readonly IPipeline _pipeline;

        private CancellationToken _cancellation = CancellationToken.None;

        public FluentConsumer(
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

        public IFluentConsumer SetCancellationToken(CancellationToken cancellation)
        {
            _cancellation = cancellation;
            return this;
        }

        public IFluentConsumer SetContextItem(string key, object value)
        {
            _contextItems.Set(key, value);
            return this;
        }

        public Task ConsumeAsync()
        {
            var filterContexts = _envelopes.Select(CreateFilterContext).ToList();

            if (filterContexts.Count == 0)
            {
                return Task.CompletedTask;
            }
            else if (filterContexts.Count == 1)
            {
                return _pipeline.ProcessAsync(filterContexts[0]);
            }
            else
            {
                return _pipeline.ProcessAsync(filterContexts);
            }
        }

        public Task<TResult> ConsumeAsync<TResult>()
        {
            var filterContexts = _envelopes.Select(CreateFilterContext).ToList();

            return _pipeline.ProcessAsync<TResult>(filterContexts);
        }

        private FilterContext CreateFilterContext(Envelope envelope)
        {
            var context = _filterContextFactory.CreateFilterContext(envelope);

            context.Cancellation = _cancellation;

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