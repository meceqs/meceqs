using System;
using Meceqs;
using Meceqs.AspNetCore;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AspNetCoreRequestPipelineBuilderExtensions
    {
        public static IPipelineBuilder UseAspNetCoreRequest(this IPipelineBuilder builder, Action<AspNetCoreRequestOptions> setupAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new AspNetCoreRequestOptions();
            setupAction?.Invoke(options);

            builder.UseFilter<AspNetCoreRequestFilter>(options);

            return builder;
        }
    }
}