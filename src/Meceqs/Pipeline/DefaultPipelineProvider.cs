using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace Meceqs.Pipeline
{
    public class DefaultPipelineProvider : IPipelineProvider
    {
        private readonly PipelineOptions _options;

        private readonly IDictionary<string, IPipeline> _pipelines = new Dictionary<string, IPipeline>();

        public DefaultPipelineProvider(IOptions<PipelineOptions> options, IServiceProvider serviceProvider)
        {
            Check.NotNull(options, nameof(options));
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            _options = options.Value;

            BuildPipelines(serviceProvider);
        }

        private void BuildPipelines(IServiceProvider serviceProvider)
        {
            foreach (var kvp in _options.Pipelines)
            {
                string pipelineName = kvp.Key;
                Action<IPipelineBuilder> setupAction = kvp.Value;

                var builder = new DefaultPipelineBuilder(serviceProvider, pipelineName);
                
                setupAction(builder);

                _pipelines.Add(pipelineName, builder.Build());
            }
        }

        public IPipeline GetPipeline(string pipelineName)
        {
            Check.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));

            IPipeline pipeline;
            if (!_pipelines.TryGetValue(pipelineName, out pipeline))
            {
                throw new ArgumentException($"Pipeline with the name '{pipelineName}' does not exist");
            }

            return pipeline;
        }
    }
}