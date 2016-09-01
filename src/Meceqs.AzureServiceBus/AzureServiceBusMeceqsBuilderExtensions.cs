using Meceqs;
using Meceqs.AzureServiceBus;
using Meceqs.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AzureServiceBusMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddAzureServiceBus(this IMeceqsBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.TryAddSingleton<IEventHubClientFactory, DefaultEventHubClientFactory>();

            return builder;
        }
    }
}