using Meceqs;
using Meceqs.AspNetCore.Consuming;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Builder
{
    public static class AspNetCoreConsumerApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAspNetCoreConsumer(this IApplicationBuilder builder, PathString? pathMatch = null)
        {
            Check.NotNull(builder, nameof(builder));

            if (!string.IsNullOrWhiteSpace(pathMatch))
            {
                builder.Map(pathMatch.Value, x => x.UseMiddleware<AspNetCoreConsumerMiddleware>());
            }
            else
            {
                builder.UseMiddleware<AspNetCoreConsumerMiddleware>();
            }

            return builder;
        }
    }
}