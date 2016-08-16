using System;
using Meceqs;
using Meceqs.Configuration;

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

            return new MeceqsBuilder(services);
        }
    }
}