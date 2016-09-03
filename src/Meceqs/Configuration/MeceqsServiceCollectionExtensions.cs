using Meceqs;
using Meceqs.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MeceqsServiceCollectionExtensions
    {
        /// <summary>
        /// Adds core services and returns a builder that allows to add further Meceqs-specific functionality.
        /// </summary>
        public static IMeceqsBuilder AddMeceqs(this IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));

            var builder = new MeceqsBuilder(services);

            // Add common services from this library
            builder.AddSerialization();
            builder.AddTypedHandling();

            return builder;
        }
    }
}