using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Configuration
{
    public class MeceqsBuilder : IMeceqsBuilder
    {
        public IServiceCollection Services { get; }

        public MeceqsBuilder(IServiceCollection services)
        {
            Guard.NotNull(services, nameof(services));

            Services = services;
        }
    }
}