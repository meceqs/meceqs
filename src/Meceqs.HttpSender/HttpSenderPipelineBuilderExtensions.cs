using Meceqs;
using Meceqs.HttpSender;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HttpSenderPipelineBuilderExtensions
    {
        public static IPipelineBuilder RunHttpSender(this IPipelineBuilder pipeline)
        {
            Guard.NotNull(pipeline, nameof(pipeline));

            pipeline.UseMiddleware<HttpSenderMiddleware>();

            return pipeline;
        }
    }
}