using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Pipeline
{
    public abstract class FilterContextBuilder<TBuilder> : IFilterContextBuilder<TBuilder>
        where TBuilder : IFilterContextBuilder<TBuilder>
    {
        private readonly IFilterContextFactory _filterContextFactory;
        private readonly IPipelineProvider _pipelineProvider;

        /// <summary>
        /// In most cases we send just a single envelope. To make sure we don't have to wrap it in a list
        /// every time, this property always holds the single/first envelope (if there is one).
        /// </summary>
        protected Envelope FirstEnvelope { get; }

        /// <summary>
        /// Contains the 2nd, 3rd... envelope. <c>null</c> if there's no or only one envelope!
        /// </summary>
        protected List<Envelope> AdditionalEnvelopes { get; }

        protected CancellationToken Cancellation { get; private set; }
        protected FilterContextItems ContextItems { get; private set; }
        protected string PipelineName { get; private set; }
        protected IServiceProvider RequestServices { get; private set; }
        protected ClaimsPrincipal User { get; private set; }

        /// <summary>
        /// Returning "this" in methods is not possible because "TBuilder" is not a derived type from this.
        /// </summary>
        public abstract TBuilder Instance { get; }

        protected FilterContextBuilder(string defaultPipelineName, Envelope envelope, IServiceProvider serviceProvider)
            : this(defaultPipelineName, serviceProvider)
        {
            Check.NotNull(envelope, nameof(envelope));

            FirstEnvelope = envelope;
        }

        protected FilterContextBuilder(string defaultPipelineName, IEnumerable<Envelope> envelopes, IServiceProvider serviceProvider)
            : this(defaultPipelineName, serviceProvider)
        {
            Check.NotNull(envelopes, nameof(envelopes));

            // Store the first envelope in the main property and the rest in a separate list.
            // If we get a list, it's possible that there's no envelope at all.
            // This will result in a no-op send!

            foreach (var envelope in envelopes)
            {
                if (FirstEnvelope == null)
                {
                    FirstEnvelope = envelope;
                }
                else
                {
                    if (AdditionalEnvelopes == null)
                    {
                        AdditionalEnvelopes = new List<Envelope>();
                    }

                    AdditionalEnvelopes.Add(envelope);
                }
            }
        }

        private FilterContextBuilder(string defaultPipelineName, IServiceProvider serviceProvider)
        {
            Check.NotNullOrWhiteSpace(defaultPipelineName, nameof(defaultPipelineName));
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            _filterContextFactory = serviceProvider.GetRequiredService<IFilterContextFactory>();
            _pipelineProvider = serviceProvider.GetRequiredService<IPipelineProvider>();

            RequestServices = serviceProvider;
            PipelineName = defaultPipelineName;
        }

        public TBuilder SetCancellationToken(CancellationToken cancellation)
        {
            Cancellation = cancellation;
            return Instance;
        }

        public TBuilder SetContextItem(string key, object value)
        {
            if (ContextItems == null)
            {
                ContextItems = new FilterContextItems();
            }

            ContextItems.Set(key, value);
            return Instance;
        }

        public TBuilder SetRequestServices(IServiceProvider requestServices)
        {
            Check.NotNull(requestServices, nameof(requestServices));

            RequestServices = requestServices;
            return Instance;
        }

        public TBuilder SetUser(ClaimsPrincipal user)
        {
            User = user;
            return Instance;
        }

        public TBuilder UsePipeline(string pipelineName)
        {
            Check.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));

            PipelineName = pipelineName;
            return Instance;
        }

        public TBuilder SetHeader(string headerName, object value)
        {
            if (FirstEnvelope != null)
            {
                FirstEnvelope.Headers[headerName] = value;
            }

            if (AdditionalEnvelopes != null)
            {
                foreach (var envelope in AdditionalEnvelopes)
                {
                    envelope.Headers[headerName] = value;
                }
            }

            return Instance;
        }

        protected virtual FilterContext CreateFilterContext(Envelope envelope, Type resultType)
        {
            Check.NotNull(envelope, nameof(envelope));

            var context = _filterContextFactory.CreateFilterContext(envelope);

            context.Initialize(PipelineName, RequestServices, resultType);

            context.Cancellation = Cancellation;
            context.User = User;

            if (ContextItems != null)
            {
                context.Items.Add(ContextItems);
            }

            return context;
        }

        protected async Task InvokePipelineAsync()
        {
            var pipeline = _pipelineProvider.GetPipeline(PipelineName);

            if (FirstEnvelope != null)
            {
                var filterContext = CreateFilterContext(FirstEnvelope, resultType: null);
                await pipeline.InvokeAsync(filterContext);
            }

            if (AdditionalEnvelopes != null)
            {
                foreach (var envelope in AdditionalEnvelopes)
                {
                    var filterContext = CreateFilterContext(envelope, resultType: null);
                    await pipeline.InvokeAsync(filterContext);
                }
            }
        }

        protected async Task<TResult> InvokePipelineAsync<TResult>()
        {
            EnsureExactlyOneEnvelope();

            var pipeline = _pipelineProvider.GetPipeline(PipelineName);

            var filterContext = CreateFilterContext(FirstEnvelope, typeof(TResult));

            await pipeline.InvokeAsync(filterContext);

            return (TResult)filterContext.Result;
        }

        protected async Task<object> InvokePipelineAsync(Type resultType)
        {
            EnsureExactlyOneEnvelope();

            var pipeline = _pipelineProvider.GetPipeline(PipelineName);

            var filterContext = CreateFilterContext(FirstEnvelope, resultType);

            await pipeline.InvokeAsync(filterContext);

            return filterContext.Result;
        }

        private void EnsureExactlyOneEnvelope()
        {
            if (FirstEnvelope == null)
            {
                throw new InvalidOperationException(
                    $"'{nameof(InvokePipelineAsync)}' with a result-type can only be called if there's exactly one envelope. " +
                    $"Actual Count: 0");
            }

            if (AdditionalEnvelopes != null)
            {
                throw new InvalidOperationException(
                    $"'{nameof(InvokePipelineAsync)}' with a result-type can only be called if there's exactly one envelope. " +
                    $"Actual Count: {1 + AdditionalEnvelopes.Count}");
            }
        }
    }
}