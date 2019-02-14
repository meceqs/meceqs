using System;

namespace Meceqs.Pipeline
{
    /// <summary>
    /// A <see cref="IPipelineProvider"/> is responsible for managing all existing pipelines.
    /// </summary>
    public interface IPipelineProvider
    {
        /// <summary>
        /// Returns the <see cref="IPipeline"/> for a given <paramref name="messageType"/>.
        /// </summary>
        /// <param name="messageType">The type of the message for which a pipeline should be returned.</param>
        /// <param name="forcedPipelineName">Don't resolve the pipeline by type and use this pipeline instead.</param>
        /// <param name="fallbackPipelineName">The pipeline to be used if no mapping exists for the message type.</param>
        /// <returns></returns>
        IPipeline GetPipeline(Type messageType, string forcedPipelineName, string fallbackPipelineName);

        /// <summary>
        /// Returns the <see cref="IPipeline"/> with the given <paramref name="pipelineName"/>.
        /// </summary>
        IPipeline GetPipeline(string pipelineName);
    }
}
