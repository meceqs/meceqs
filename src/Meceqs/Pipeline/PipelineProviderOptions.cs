using System;
using System.Collections.Generic;

namespace Meceqs.Pipeline
{
    public class PipelineProviderOptions
    {
        private readonly Dictionary<string, Action<IPipelineBuilder>> _pipelines = new Dictionary<string, Action<IPipelineBuilder>>();

        public IReadOnlyDictionary<string, Action<IPipelineBuilder>> Pipelines => _pipelines;

        public void AddPipeline(string name, Action<IPipelineBuilder> builder)
        {
            Guard.NotNullOrWhiteSpace(name, nameof(name));
            Guard.NotNull(builder, nameof(builder));

            if (_pipelines.ContainsKey(name))
            {
                throw new InvalidOperationException("Pipeline already exists: " + name);
            }

            _pipelines.Add(name, builder);
        }
    }
}
