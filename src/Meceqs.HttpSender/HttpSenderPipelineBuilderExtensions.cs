using Meceqs;
using Meceqs.HttpSender;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HttpSenderPipelineBuilderExtensions
    {
        public static IPipelineBuilder RunHttpSender(this IPipelineBuilder pipeline)
        {
            Check.NotNull(pipeline, nameof(pipeline));

            pipeline.UseFilter<HttpSenderFilter>();

            return pipeline;
        }
    }
}