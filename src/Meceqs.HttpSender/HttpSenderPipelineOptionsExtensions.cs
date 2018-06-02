using Meceqs;
using Meceqs.HttpSender;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HttpSenderPipelineOptionsExtensions
    {
        public static PipelineOptions RunHttpSender(this PipelineOptions pipeline)
        {
            Guard.NotNull(pipeline, nameof(pipeline));

            pipeline.UseMiddleware<HttpSenderMiddleware>();

            return pipeline;
        }
    }
}