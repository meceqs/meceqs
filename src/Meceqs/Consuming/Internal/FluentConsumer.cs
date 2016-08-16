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
        private readonly IPipelineProvider _pipelineProvider;

        private CancellationToken _cancellation = CancellationToken.None;
        private string _pipelineName = ConsumeOptions.DefaultPipelineName;

        public FluentConsumer(
            IList<Envelope> envelopes,
            IFilterContextFactory filterContextFactory,
            IPipelineProvider pipelineProvider)
        {
            Check.NotNull(envelopes, nameof(envelopes));
            Check.NotNull(filterContextFactory, nameof(filterContextFactory));
            Check.NotNull(pipelineProvider, nameof(pipelineProvider));

            _envelopes = envelopes;
            _filterContextFactory = filterContextFactory;
            _pipelineProvider = pipelineProvider;
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

        public IFluentConsumer UsePipeline(string pipelineName)
        {
            Check.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));

            _pipelineName = pipelineName;
            return this;
        }

        public Task ConsumeAsync()
        {
            var filterContexts = _envelopes.Select(CreateFilterContext).ToList();
            var pipeline = _pipelineProvider.GetPipeline(_pipelineName);

            return pipeline.ProcessAsync(filterContexts);
        }

        public Task<TResult> ConsumeAsync<TResult>()
        {
            var filterContexts = _envelopes.Select(CreateFilterContext).ToList();
            var pipeline = _pipelineProvider.GetPipeline(_pipelineName);
            
            return pipeline.ProcessAsync<TResult>(filterContexts);
        }

        private FilterContext CreateFilterContext(Envelope envelope)
        {
            var context = _filterContextFactory.CreateFilterContext(envelope);

            context.Cancellation = _cancellation;
            context.Items.Add(_contextItems);

            return context;
        }
    }
}