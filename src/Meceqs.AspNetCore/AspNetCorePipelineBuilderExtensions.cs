using Meceqs;
using Meceqs.AspNetCore;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AspNetCorePipelineBuilderExtensions
    {
        public static IPipelineBuilder UseAspNetCore(this IPipelineBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.UseFilter<AspNetCoreFilter>();

            return builder;
        }
    }
}