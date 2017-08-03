using System;
using Meceqs;
using Meceqs.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting
{
    public static class MeceqsWebHostBuilderExtensions
    {
        public static IWebHostBuilder ConfigureMeceqs(this IWebHostBuilder webHostBuilder, Action<IMeceqsBuilder> configure)
        {
            Guard.NotNull(webHostBuilder, nameof(webHostBuilder));
            Guard.NotNull(configure, nameof(configure));

            return ConfigureMeceqs(webHostBuilder, (_, meceqsBuilder) => configure(meceqsBuilder));
        }

        public static IWebHostBuilder ConfigureMeceqs(this IWebHostBuilder webHostBuilder, Action<WebHostBuilderContext, IMeceqsBuilder> configure)
        {
            Guard.NotNull(webHostBuilder, nameof(webHostBuilder));
            Guard.NotNull(configure, nameof(configure));

            webHostBuilder.ConfigureServices((context, services) =>
            {
                var meceqsBuilder = services.AddMeceqs();

                // Default configuration
                meceqsBuilder.AddAspNetCore();

                configure(context, meceqsBuilder);
            });

            return webHostBuilder;
        }
    }
}