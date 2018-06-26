using Meceqs;
using Meceqs.AspNetCore.Receiving;

namespace Microsoft.AspNetCore.Builder
{
    public static class AspNetCoreApplicationBuilderExtensions
    {
        /// <summary>
        /// Registers all Meceqs endpoints in the ASP.NET Core request pipeline.
        /// </summary>
        public static IApplicationBuilder UseMeceqs(this IApplicationBuilder builder)
        {
            Guard.NotNull(builder, nameof(builder));

            builder.UseMiddleware<ReceiveEndpointMiddleware>();

            return builder;
        }
    }
}