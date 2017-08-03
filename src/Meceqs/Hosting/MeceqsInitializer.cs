using Meceqs.Pipeline;

namespace Meceqs.Hosting
{
    public class MeceqsInitializer
    {
        private readonly IPipelineProvider _pipelineProvider;

        public MeceqsInitializer(IPipelineProvider pipelineProvider)
        {
            Guard.NotNull(pipelineProvider, nameof(pipelineProvider));

            _pipelineProvider = pipelineProvider;
        }

        public void Start()
        {
            _pipelineProvider.BuildPipelines();
        }
    }
}