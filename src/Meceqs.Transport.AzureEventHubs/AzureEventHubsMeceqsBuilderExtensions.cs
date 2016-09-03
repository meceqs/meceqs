using Meceqs;
using Meceqs.Configuration;
using Meceqs.Transport.AzureEventHubs;
using Meceqs.Transport.AzureEventHubs.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AzureEventHubsMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddAzureEventHubs(this IMeceqsBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.TryAddSingleton<IEventHubConsumer, DefaultEventHubConsumer>();
            builder.Services.TryAddSingleton<IEventDataConverter, DefaultEventDataConverter>();
            builder.Services.TryAddSingleton<IEventHubClientFactory, DefaultEventHubClientFactory>();

            return builder;
        }
    }
}