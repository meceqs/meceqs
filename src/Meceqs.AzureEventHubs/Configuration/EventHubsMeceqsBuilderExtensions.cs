using System;
using Meceqs;
using Meceqs.AzureEventHubs.Configuration;
using Meceqs.AzureEventHubs.Consuming;
using Meceqs.AzureEventHubs.Internal;
using Meceqs.AzureEventHubs.Sending;
using Meceqs.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventHubsMeceqsBuilderExtensions
    {
        private static IMeceqsBuilder AddEventHubServices(this IMeceqsBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.TryAddSingleton<IEventDataConverter, DefaultEventDataConverter>();
            builder.Services.TryAddSingleton<IEventHubClientFactory, DefaultEventHubClientFactory>();
            builder.Services.TryAddSingleton<IEventHubConsumer, DefaultEventHubConsumer>();

            return builder;
        }

        /// <summary>
        /// Adds an Azure Event Hubs consumer pipeline.
        /// </summary>
        public static IMeceqsBuilder AddEventHubConsumer(
            this IMeceqsBuilder builder,
            Action<IEventHubConsumerBuilder> consumer)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(consumer, nameof(consumer));

            var consumerBuilder = new EventHubConsumerBuilder();
            consumer?.Invoke(consumerBuilder);

            builder.AddEventHubServices();

            foreach (var assembly in consumerBuilder.GetDeserializationAssemblies())
            {
                builder.AddDeserializationAssembly(assembly);
            }

            var consumerOptions = consumerBuilder.GetConsumerOptions();
            if (consumerOptions != null)
            {
                builder.Services.Configure(consumerOptions);
            }

            builder.AddPipeline(consumerBuilder.GetPipelineName(), consumerBuilder.GetPipeline());

            return builder;
        }

        /// <summary>
        /// Adds an Azure Event Hubs sender pipeline.
        /// </summary>
        public static IMeceqsBuilder AddEventHubSender(this IMeceqsBuilder builder, Action<IEventHubSenderBuilder> sender)
        {
            return AddEventHubSender(builder, null, sender);
        }

        /// <summary>
        /// Adds an Azure Event Hubs sender pipeline.
        /// </summary>
        public static IMeceqsBuilder AddEventHubSender(
            this IMeceqsBuilder builder,
            IConfiguration configuration,
            Action<IEventHubSenderBuilder> sender)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(sender, nameof(sender));

            if (configuration != null)
            {
                builder.Services.Configure<EventHubSenderOptions>(configuration);
            }

            var senderBuilder = new EventHubSenderBuilder();
            sender?.Invoke(senderBuilder);

            builder.AddEventHubServices();

            var senderOptions = senderBuilder.GetSenderOptions();
            if (senderOptions != null)
            {
                builder.Services.Configure(senderOptions);
            }

            builder.AddPipeline(senderBuilder.GetPipelineName(), senderBuilder.GetPipeline());

            return builder;
        }
    }
}