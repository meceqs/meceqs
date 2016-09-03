using Meceqs;
using Meceqs.Transport.AzureServiceBus;
using Meceqs.Transport.AzureServiceBus.Internal;
using Meceqs.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AzureServiceBusMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddAzureServiceBus(this IMeceqsBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            // Service Bus
            builder.Services.TryAddSingleton<IServiceBusConsumer, DefaultServiceBusConsumer>();
            builder.Services.TryAddSingleton<IBrokeredMessageConverter, DefaultBrokeredMessageConverter>();

            return builder;
        }
    }
}