namespace Meceqs.Pipeline
{
    /// <summary>
    /// A <see cref="IPipelineProvider"/> is responsible for managing all existing pipelines.
    /// </summary>
    public interface IPipelineProvider
    {
        /// <summary>
        /// Implementations might decide to build pipelines lazily. This call will force the creation
        /// and is useful when you want to ensure the creation/validation of pipelines on application startup.
        /// </summary>
        void BuildPipelines();

        /// <summary>
        /// Returns the <see cref="IPipeline"/> with the given <paramref name="pipelineName"/>.
        /// </summary>
        IPipeline GetPipeline(string pipelineName);
    }
}