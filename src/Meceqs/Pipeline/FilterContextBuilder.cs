using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Pipeline
{
    public abstract class FilterContextBuilder<TBuilder> : IFilterContextBuilder<TBuilder>
        where TBuilder : IFilterContextBuilder<TBuilder>
    {
        private readonly IFilterContextFactory _filterContextFactory;
        private readonly IPipelineProvider _pipelineProvider;

        protected IList<Envelope> Envelopes { get; }
        protected CancellationToken Cancellation { get; private set; }
        protected FilterContextItems ContextItems { get; } = new FilterContextItems();
        protected string PipelineName { get; private set; }
        protected IServiceProvider RequestServices { get; private set; }

        protected FilterContextBuilder(
            string defaultPipelineName,
            IList<Envelope> envelopes,
            IFilterContextFactory filterContextFactory,
            IPipelineProvider pipelineProvider)
        {
            Check.NotNullOrWhiteSpace(defaultPipelineName, nameof(defaultPipelineName));
            Check.NotNull(envelopes, nameof(envelopes));
            Check.NotNull(filterContextFactory, nameof(filterContextFactory));
            Check.NotNull(pipelineProvider, nameof(pipelineProvider));

            PipelineName = defaultPipelineName;

            _filterContextFactory = filterContextFactory;
            _pipelineProvider = pipelineProvider;
        }

        public TBuilder SetCancellationToken(CancellationToken cancellation)
        {
            Cancellation = cancellation;
            return GetInstance();
        }

        public TBuilder SetContextItem(string key, object value)
        {
            ContextItems.Set(key, value);
            return GetInstance();
        }

        public TBuilder SetRequestServices(IServiceProvider requestServices)
        {
            RequestServices = requestServices;
            return GetInstance();
        }

        public TBuilder UsePipeline(string pipelineName)
        {
            Check.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));

            PipelineName = pipelineName;
            return GetInstance();
        }

        protected abstract TBuilder GetInstance();

        protected virtual FilterContext CreateFilterContext(Envelope envelope)
        {
            var context = _filterContextFactory.CreateFilterContext(envelope);

            context.Cancellation = Cancellation;
            context.RequestServices = RequestServices;
            context.Items.Add(ContextItems);

            return context;
        }

        protected async Task ProcessAsync()
        {
            var pipeline = _pipelineProvider.GetPipeline(PipelineName);

            foreach (var envelope in Envelopes)
            {
                var filterContext = CreateFilterContext(envelope);
                await pipeline.ProcessAsync(filterContext);
            }
        }

        public Task<TResult> ProcessAsync<TResult>(
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            if (Envelopes.Count == 1)
            {
                var filterContext = CreateFilterContext(Envelopes[0]);
                var pipeline = _pipelineProvider.GetPipeline(PipelineName);

                return pipeline.ProcessAsync<TResult>(filterContext);
            }

            throw new InvalidOperationException(
                $"'{memberName}' with a result-type can only be called if there's exactly one envelope. " +
                $"Actual Count: {Envelopes.Count}");
        }
    }
}