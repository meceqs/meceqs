using Meceqs;
using Meceqs.AspNetCore.Receiving;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Builder
{
    public static class AspNetCoreReceiverApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAspNetCoreReceiver(this IApplicationBuilder builder, PathString? pathMatch = null)
        {
            Check.NotNull(builder, nameof(builder));

            if (!string.IsNullOrWhiteSpace(pathMatch))
            {
                builder.Map(pathMatch.Value, x => x.UseMiddleware<AspNetCoreReceiverMiddleware>());
            }
            else
            {
                builder.UseMiddleware<AspNetCoreReceiverMiddleware>();
            }

            return builder;
        }
    }
}