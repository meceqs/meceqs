namespace Meceqs.Pipeline
{
    /// <summary>
    /// A <see cref="IPipelineProvider"/> is responsible for managing all existing pipelines.
    /// </summary>
    public interface IPipelineProvider
    {
        /// <summary>
        /// Returns the <see cref="IPipeline"/> with the given <paramref name="pipelineName"/>.
        /// </summary>
        IPipeline GetPipeline(string pipelineName);
    }
}