using System;
using System.Collections.Generic;
using System.Security.Claims;
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
        protected FilterContextItems ContextItems { get; private set; }
        protected string PipelineName { get; private set; }
        protected IServiceProvider RequestServices { get; private set; }
        protected ClaimsPrincipal User { get; private set; }

        /// <summary>
        /// Returning "this" is not possible because "TBuilder" is not a derived type from this.
        /// </summary>
        public abstract TBuilder Instance { get; }

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
            Envelopes = envelopes;

            _filterContextFactory = filterContextFactory;
            _pipelineProvider = pipelineProvider;
        }

        public virtual TBuilder SetCancellationToken(CancellationToken cancellation)
        {
            Cancellation = cancellation;
            return Instance;
        }

        public virtual TBuilder SetContextItem(string key, object value)
        {
            if (ContextItems == null)
            {
                ContextItems = new FilterContextItems();
            }

            ContextItems.Set(key, value);
            return Instance;
        }

        public virtual TBuilder SetRequestServices(IServiceProvider requestServices)
        {
            RequestServices = requestServices;
            return Instance;
        }

        public virtual TBuilder SetUser(ClaimsPrincipal user)
        {
            User = user;
            return Instance;
        }

        public virtual TBuilder UsePipeline(string pipelineName)
        {
            Check.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));

            PipelineName = pipelineName;
            return Instance;
        }

        public TBuilder SetHeader(string headerName, object value)
        {
            foreach (var envelope in Envelopes)
            {
                envelope.Headers[headerName] = value;
            }

            return Instance;
        }

        protected virtual FilterContext CreateFilterContext(Envelope envelope)
        {
            Check.NotNull(envelope, nameof(envelope));

            var context = _filterContextFactory.CreateFilterContext(envelope);

            context.Cancellation = Cancellation;
            context.RequestServices = RequestServices;
            context.User = User;

            if (ContextItems != null)
            {
                context.Items.Add(ContextItems);
            }

            return context;
        }

        protected virtual async Task ProcessAsync()
        {
            var pipeline = _pipelineProvider.GetPipeline(PipelineName);

            foreach (var envelope in Envelopes)
            {
                var filterContext = CreateFilterContext(envelope);
                await pipeline.ProcessAsync(filterContext);
            }
        }

        protected virtual Task<TResult> ProcessAsync<TResult>()
        {
            if (Envelopes.Count != 1)
            {
                throw new InvalidOperationException(
                    $"'{nameof(ProcessAsync)}' with a result-type can only be called if there's exactly one envelope. " +
                    $"Actual Count: {Envelopes.Count}");
            }

            var pipeline = _pipelineProvider.GetPipeline(PipelineName);
            var filterContext = CreateFilterContext(Envelopes[0]);

            return pipeline.ProcessAsync<TResult>(filterContext);
        }

        protected virtual async Task<object> ProcessAsync(Type resultType)
        {
            if (Envelopes.Count != 1)
            {
                throw new InvalidOperationException(
                    $"'{nameof(ProcessAsync)}' with a result-type can only be called if there's exactly one envelope. " +
                    $"Actual Count: {Envelopes.Count}");
            }

            var pipeline = _pipelineProvider.GetPipeline(PipelineName);
            var filterContext = CreateFilterContext(Envelopes[0]);

            filterContext.ExpectedResultType = resultType;

            await pipeline.ProcessAsync(filterContext);

            return filterContext.Result;
        }
    }
}