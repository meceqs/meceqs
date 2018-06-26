using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.DependencyInjection
{
    public class MeceqsBuilder : IMeceqsBuilder
    {
        public IServiceCollection Services { get; }

        public IConfiguration Configuration { get; }

        internal MeceqsBuilder(IServiceCollection services, IConfiguration configuration)
        {
            Guard.NotNull(services, nameof(services));

            Services = services;

            // Ensures people don't have to deal with a nullable configuration.
            Configuration = configuration ?? new ConfigurationBuilder().Build();
        }
    }
}