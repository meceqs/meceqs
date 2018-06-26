using Meceqs;
using Meceqs.HttpSender;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HttpSenderPipelineOptionsExtensions
    {
        public static PipelineBuilder RunHttpSender(this PipelineBuilder pipeline)
        {
            Guard.NotNull(pipeline, nameof(pipeline));

            pipeline.UseMiddleware<HttpSenderMiddleware>();

            return pipeline;
        }
    }
}