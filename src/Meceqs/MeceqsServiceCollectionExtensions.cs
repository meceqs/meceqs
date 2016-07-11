using Meceqs;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MeceqsServiceCollectionExtensions
    {
        public static IServiceCollection AddMeceqs(this IServiceCollection services)
        {
            services.TryAddSingleton<IEnvelopeFactory, DefaultEnvelopeFactory>();

            services.TryAddSingleton<IEnvelopeTypeLoader, DefaultEnvelopeTypeLoader>();

            return services;
        }
    }
}