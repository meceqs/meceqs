using System.Threading;

namespace Meceqs.Pipeline
{
    public class PipelineOptions
    {
        private IPipeline _pipeline = null;
        private bool _pipelineInitialized = false;
        private object _pipelineLock = new object();

        public IPipelineBuilder Builder { get; set; }

        public string Name { get; set; }

        public IPipeline GetPipeline()
        {
            return LazyInitializer.EnsureInitialized(
                ref _pipeline,
                ref _pipelineInitialized,
                ref _pipelineLock,
                () => Builder.Build(Name)
            );
        }
    }
}