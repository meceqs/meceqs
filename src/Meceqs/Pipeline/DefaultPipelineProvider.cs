using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Meceqs.Pipeline
{
    public class DefaultPipelineProvider : IPipelineProvider
    {
        private readonly PipelineProviderOptions _providerOptions;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IMessageContextEnricher _messageContextEnricher;

        private readonly ConcurrentDictionary<string, Lazy<IPipeline>> _pipelines;
        private readonly Func<string, Lazy<IPipeline>> _pipelineFactory;

        public DefaultPipelineProvider(
            IOptions<PipelineProviderOptions> providerOptions,
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory,
            IMessageContextEnricher messageContextEnricher = null)
        {
            Guard.NotNull(providerOptions, nameof(providerOptions));
            Guard.NotNull(loggerFactory, nameof(loggerFactory));

            _providerOptions = providerOptions.Value;
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
            if (!_providerOptions.Pipelines.TryGetValue(pipelineName, out Action<IPipelineBuilder> builderAction))
            {
                throw new ArgumentException($"A pipeline with the name '{pipelineName}' has not been configured.");
            }

            var pipelineBuilder = new PipelineBuilder(_serviceProvider);
            builderAction(pipelineBuilder);

            MiddlewareDelegate pipelineDelegate = pipelineBuilder.Build();

            return new DefaultPipeline(pipelineDelegate, pipelineName, _loggerFactory, _messageContextEnricher);
        }
    }
}