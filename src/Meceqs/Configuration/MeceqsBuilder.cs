using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Configuration
{
    public class MeceqsBuilder : IMeceqsBuilder
    {
        public IServiceCollection Services { get; }

        public MeceqsBuilder(IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));

            Services = services;
        }

    }
}