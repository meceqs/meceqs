using Meceqs;
using Meceqs.Sending;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MeceqsServiceCollectionExtensions
    {
        public static IServiceCollection AddMeceqs(this IServiceCollection services)
        {
            // Common services
            services.TryAddSingleton<IEnvelopeFactory, DefaultEnvelopeFactory>();
            services.TryAddSingleton<IEnvelopeTypeLoader, DefaultEnvelopeTypeLoader>();

            // Sending
            services.TryAddSingleton<IMessageCorrelator, DefaultMessageCorrelator>();

            return services;
        }
    }
}