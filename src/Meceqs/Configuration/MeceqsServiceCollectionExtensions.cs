using System;
using Meceqs;
using Meceqs.Configuration;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MeceqsServiceCollectionExtensions
    {
        public static IMeceqsBuilder AddMeceqs(this IServiceCollection services, Action<MeceqsOptions> setupAction = null)
        {
            Check.NotNull(services, nameof(services));

            if (setupAction != null)
            {
                services.Configure<MeceqsOptions>(setupAction);
            }

            // Pipeline
            services.AddSingleton<IFilterContextFactory, DefaultFilterContextFactory>();
            services.AddTransient<IPipelineBuilder, DefaultPipelineBuilder>();

            return new MeceqsBuilder(services);
        }
    }
}