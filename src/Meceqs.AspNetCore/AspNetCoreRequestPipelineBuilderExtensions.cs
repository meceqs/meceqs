using Meceqs;
using Meceqs.AspNetCore;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AspNetCoreRequestPipelineBuilderExtensions
    {
        public static IPipelineBuilder UseAspNetCoreRequest(this IPipelineBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.UseFilter<AspNetCoreRequestFilter>();

            return builder;
        }
    }
}