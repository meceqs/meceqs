using System.Threading;

namespace Meceqs.Pipeline
{
    public class PipelineOptions
    {
        private FilterDelegate _pipeline = null;
        private bool _pipelineInitialized = false;
        private object _pipelineLock = new object();

        public IPipelineBuilder Builder { get; set; }

        public FilterDelegate Pipeline
        {
            get
            {
                return LazyInitializer.EnsureInitialized(
                    ref _pipeline,
                    ref _pipelineInitialized,
                    ref _pipelineLock,
                    Builder.Build
                );
            }
        }
    }
}