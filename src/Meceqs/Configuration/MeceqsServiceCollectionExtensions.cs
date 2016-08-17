using Meceqs;
using Meceqs.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MeceqsServiceCollectionExtensions
    {
        public static IMeceqsBuilder AddMeceqs(this IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));

            var builder = new MeceqsBuilder(services);

            // Add common services from this library
            builder.AddTypedHandling();

            return builder;
        }
    }
}