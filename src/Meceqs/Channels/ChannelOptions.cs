using System.Threading;
using Meceqs.Pipeline;

namespace Meceqs.Channels
{
    public class ChannelOptions
    {
        private FilterDelegate _pipeline = null;
        private bool _pipelineInitialized = false;
        private object _pipelineLock = new object();

        public IPipelineBuilder PipelineBuilder { get; set; }

        public FilterDelegate Pipeline
        {
            get
            {
                return LazyInitializer.EnsureInitialized(
                    ref _pipeline,
                    ref _pipelineInitialized,
                    ref _pipelineLock,
                    PipelineBuilder.Build
                );
            }
        }
    }
}