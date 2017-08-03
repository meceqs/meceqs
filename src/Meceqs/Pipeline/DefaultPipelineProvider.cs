using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Meceqs.Pipeline
{
    public class DefaultPipelineProvider : IPipelineProvider
    {
        private readonly IDictionary<string, IPipeline> _pipelines = new Dictionary<string, IPipeline>();

        public DefaultPipelineProvider(
            IOptions<PipelineOptions> options,
            IServiceProvider serviceProvider)
        {
            Guard.NotNull(options, nameof(options));
            Guard.NotNull(serviceProvider, nameof(serviceProvider));

            BuildPipelines(serviceProvider, options.Value);
        }

        public void BuildPipelines()
        {
            // No-op because pipelines are already built in the constructor.
            // If we would do it later, it would have to be thread-safe
            // and use e.g. Lazy<> or custom locking.
        }

        private void BuildPipelines(IServiceProvider serviceProvider, PipelineOptions options)
        {
            foreach (var kvp in options.Pipelines)
            {
                string pipelineName = kvp.Key;
                Action<IPipelineBuilder> setupAction = kvp.Value;

                var builder = serviceProvider.GetRequiredService<IPipelineBuilder>();

                setupAction(builder);

                _pipelines.Add(pipelineName, builder.Build(pipelineName));
            }
        }

        public IPipeline GetPipeline(string pipelineName)
        {
            Guard.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));

            IPipeline pipeline;
            if (!_pipelines.TryGetValue(pipelineName, out pipeline))
            {
                throw new ArgumentException($"Pipeline with the name '{pipelineName}' does not exist");
            }

            return pipeline;
        }
    }
}