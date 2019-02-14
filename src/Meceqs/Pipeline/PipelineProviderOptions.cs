using System;
using System.Collections.Generic;

namespace Meceqs.Pipeline
{
    public class PipelineProviderOptions
    {
        private readonly Dictionary<Type, string> _mappings = new Dictionary<Type, string>();
        private readonly Dictionary<string, Action<IPipelineBuilder>> _pipelines = new Dictionary<string, Action<IPipelineBuilder>>();

        public IReadOnlyDictionary<Type, string> Mappings => _mappings;
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

        public void AddMessageTypeMapping(Type messageType, string pipelineName)
        {
            Guard.NotNull(messageType, nameof(messageType));
            Guard.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));

            if (_mappings.TryGetValue(messageType, out string existingPipelineName))
            {
                throw new InvalidOperationException(
                    $"The mapping '{messageType.FullName}->{pipelineName}' could not be added because " +
                    $"there already exists a mapping for this message type to the pipeline '{existingPipelineName}'.");
            }

            _mappings.Add(messageType, pipelineName);
        }
    }
}
