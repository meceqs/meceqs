using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Meceqs.Pipeline
{
    public class DefaultPipelineProvider : IPipelineProvider
    {
        private readonly IOptionsMonitor<PipelineOptions> _optionsMonitor;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IMessageContextEnricher _messageContextEnricher;

        private readonly ConcurrentDictionary<string, Lazy<IPipeline>> _pipelines;
        private readonly Func<string, Lazy<IPipeline>> _pipelineFactory;

        public DefaultPipelineProvider(
            IOptionsMonitor<PipelineOptions> optionsMonitor,
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory,
            IMessageContextEnricher messageContextEnricher = null)
        {
            Guard.NotNull(optionsMonitor, nameof(optionsMonitor));
            Guard.NotNull(loggerFactory, nameof(loggerFactory));

            _optionsMonitor = optionsMonitor;
            _serviceProvider = serviceProvider;
            _loggerFactory = loggerFactory;
            _messageContextEnricher = messageContextEnricher;

            // case-sensitive because named options is.
            _pipelines = new ConcurrentDictionary<string, Lazy<IPipeline>>(StringComparer.Ordinal);
            _pipelineFactory = (name) =>
            {
                return new Lazy<IPipeline>(() =>
                {
                    return CreatePipeline(name);
                }, LazyThreadSafetyMode.ExecutionAndPublication);
            };
        }

        public IPipeline GetPipeline(string pipelineName)
        {
            Guard.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));

            IPipeline pipeline = _pipelines.GetOrAdd(pipelineName, _pipelineFactory).Value;

            return pipeline;
        }

        private IPipeline CreatePipeline(string pipelineName)
        {
            var options = _optionsMonitor.Get(pipelineName);

            MiddlewareDelegate pipelineDelegate = options.BuildPipeline(_serviceProvider);

            // TODO Is it safe to throw in a Lazy-initializer?
            if (pipelineDelegate == null)
            {
                throw new ArgumentException($"A pipeline with the name '{pipelineName}' has not been configured.");
            }

            return new DefaultPipeline(pipelineDelegate, pipelineName, _loggerFactory, _messageContextEnricher);
        }
    }
}