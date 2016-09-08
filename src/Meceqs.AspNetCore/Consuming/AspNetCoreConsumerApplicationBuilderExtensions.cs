using System;
using Meceqs;
using Meceqs.AspNetCore.Consuming;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder
{
    public static class AspNetCoreConsumerApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAspNetCoreConsumer(this IApplicationBuilder builder, Action<AspNetCoreConsumerOptions> options)
        {
            return UseAspNetCoreConsumer(builder, null, options);
        }

        public static IApplicationBuilder UseAspNetCoreConsumer(this IApplicationBuilder builder, PathString pathMatch, Action<AspNetCoreConsumerOptions> options)
        {
            var consumerOptions = new AspNetCoreConsumerOptions();
            options?.Invoke(consumerOptions);

            return UseAspNetCoreConsumer(builder, pathMatch, consumerOptions);
        }

        public static IApplicationBuilder UseAspNetCoreConsumer(this IApplicationBuilder builder, AspNetCoreConsumerOptions options = null)
        {
            return UseAspNetCoreConsumer(builder, null, options);
        }

        public static IApplicationBuilder UseAspNetCoreConsumer(this IApplicationBuilder builder, PathString pathMatch, AspNetCoreConsumerOptions options = null)
        {
            Check.NotNull(builder, nameof(builder));

            var optionsWrapper = Options.Create(options);

            if (!string.IsNullOrWhiteSpace(pathMatch))
            {
                builder.Map(pathMatch, x => x.UseMiddleware<AspNetCoreConsumerMiddleware>(optionsWrapper));
            }
            else
            {
                builder.UseMiddleware<AspNetCoreConsumerMiddleware>(optionsWrapper);
            }

            return builder;
        }
    }
}