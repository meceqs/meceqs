using Meceqs;
using Meceqs.HttpSender;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HttpSenderPipelineBuilderExtensions
    {
        public static void RunHttpSender(this IPipelineBuilder pipeline)
        {
            Guard.NotNull(pipeline, nameof(pipeline));

            pipeline.UseMiddleware<HttpSenderMiddleware>();
        }
    }
}