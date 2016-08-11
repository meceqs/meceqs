using System;
using Meceqs;
using Meceqs.DependencyInjection;
using Meceqs.Filters.TypedHandling;
using Meceqs.Filters.TypedHandling.Internal;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MeceqsServiceCollectionExtensions
    {
        public static IMeceqsBuilder AddMeceqs(this IServiceCollection services, Action<MeceqsOptions> setupAction)
        {
            Check.NotNull(services, nameof(services));

            if (setupAction != null)
            {
                services.Configure<MeceqsOptions>(setupAction);
            }

            return services.AddMeceqs();
        }

        public static IMeceqsBuilder AddMeceqs(this IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));

            // TODO @cweiss Options?
            // TODO @cweiss check lifecycles!

            // Core
            services.AddTransient<IEnvelopeFactory, DefaultEnvelopeFactory>();

            // Pipeline
            services.AddSingleton<IFilterContextFactory, DefaultFilterContextFactory>();
            services.AddTransient<IPipelineBuilder, DefaultPipelineBuilder>();

            return new MeceqsBuilder(services);
        }
    }
}