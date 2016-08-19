using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Meceqs.Sending.Internal
{
    public class FluentSender : IFluentSender
    {
        private readonly IList<Envelope> _envelopes;
        private readonly FilterContextItems _contextItems = new FilterContextItems();
        private readonly IEnvelopeCorrelator _envelopeCorrelator;
        private readonly IFilterContextFactory _filterContextFactory;
        private readonly IPipelineProvider _pipelineProvider;

        private CancellationToken _cancellation = CancellationToken.None;
        private string _pipelineName = SendOptions.DefaultPipelineName;

        public FluentSender(
            IList<Envelope> envelopes,
            IEnvelopeCorrelator envelopeCorrelator,
            IFilterContextFactory filterContextFactory,
            IPipelineProvider pipelineProvider)
        {
            Check.NotNull(envelopes, nameof(envelopes));
            Check.NotNull(envelopeCorrelator, nameof(envelopeCorrelator));
            Check.NotNull(filterContextFactory, nameof(filterContextFactory));
            Check.NotNull(pipelineProvider, nameof(pipelineProvider));

            _envelopes = envelopes;
            _envelopeCorrelator = envelopeCorrelator;
            _filterContextFactory = filterContextFactory;
            _pipelineProvider = pipelineProvider;
        }

        public IFluentSender CorrelateWith(Envelope source)
        {
            foreach (var envelope in _envelopes)
            {
                _envelopeCorrelator.CorrelateSourceWithTarget(source, envelope);
            }

            return this;
        }

        public IFluentSender SetCancellationToken(CancellationToken cancellation)
        {
            _cancellation = cancellation;
            return this;
        }

        public IFluentSender SetHeader(string headerName, object value)
        {
            foreach (var envelope in _envelopes)
            {
                envelope.Headers[headerName] = value;
            }

            return this;
        }

        public IFluentSender SetContextItem(string key, object value)
        {
            _contextItems.Set(key, value);
            return this;
        }

        public IFluentSender UsePipeline(string pipelineName)
        {
            Check.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));

            _pipelineName = pipelineName;
            return this;
        }

        public async Task SendAsync()
        {
            var filterContexts = _envelopes.Select(CreateFilterContext).ToList();
            var pipeline = _pipelineProvider.GetPipeline(_pipelineName);

            foreach (var context in filterContexts)
            {
                await pipeline.ProcessAsync(context);
            }
        }

        public Task<TResult> SendAsync<TResult>()
        {
            var filterContexts = _envelopes.Select(CreateFilterContext).ToList();
            var pipeline = _pipelineProvider.GetPipeline(_pipelineName);

            if (filterContexts.Count == 1)
            {
                return pipeline.ProcessAsync<TResult>(filterContexts[0]);
            }
            
            throw new InvalidOperationException(
                $"'{nameof(SendAsync)}' with a result-type can only be called if there's exactly one envelope. " +
                $"Actual Count: {filterContexts.Count}");
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