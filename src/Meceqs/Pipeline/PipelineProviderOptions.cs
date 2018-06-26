using System;
using System.Collections.Generic;

namespace Meceqs.Pipeline
{
    public class PipelineProviderOptions
    {
        private readonly Dictionary<string, PipelineBuilder> _pipelines = new Dictionary<string, PipelineBuilder>();

        public IReadOnlyDictionary<string, PipelineBuilder> Pipelines => _pipelines;

        public void AddPipeline(string name, Action<PipelineBuilder> builder)
        {
            Guard.NotNullOrWhiteSpace(name, nameof(name));
            Guard.NotNull(builder, nameof(builder));

            var builderInstance = new PipelineBuilder(name);
            builder(builderInstance);

            AddPipeline(builderInstance);
        }

        public void AddPipeline(PipelineBuilder builder)
        {
            Guard.NotNull(builder, nameof(builder));

            if (_pipelines.ContainsKey(builder.Name))
            {
                throw new InvalidOperationException("Pipeline already exists: " + builder.Name);
            }

            _pipelines.Add(builder.Name, builder);
        }
    }
}
