namespace Meceqs.Pipeline
{
    public interface IPipelineProvider
    {
        IPipeline GetPipeline(string pipelineName);
    }
}