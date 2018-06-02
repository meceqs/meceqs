using System;
using Meceqs;
using Meceqs.AzureEventHubs.Configuration;
using Meceqs.AzureEventHubs.Internal;
using Meceqs.AzureEventHubs.Receiving;
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
            Guard.NotNull(builder, nameof(builder));

            builder.Services.TryAddSingleton<IEventDataConverter, DefaultEventDataConverter>();
            builder.Services.TryAddSingleton<IEventHubClientFactory, DefaultEventHubClientFactory>();
            builder.Services.TryAddSingleton<IEventHubReceiver, DefaultEventHubReceiver>();

            return builder;
        }

        /// <summary>
        /// Adds an Azure Event Hubs receiver pipeline.
        /// </summary>
        public static IMeceqsBuilder AddEventHubReceiver(
            this IMeceqsBuilder builder,
            Action<IEventHubReceiverBuilder> receiver)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNull(receiver, nameof(receiver));

            var receiverBuilder = new EventHubReceiverBuilder();
            receiver?.Invoke(receiverBuilder);

            builder.AddEventHubServices();

            foreach (var assembly in receiverBuilder.GetDeserializationAssemblies())
            {
                builder.AddDeserializationAssembly(assembly);
            }

            var receiverOptions = receiverBuilder.GetReceiverOptions();
            if (receiverOptions != null)
            {
                builder.Services.Configure(receiverOptions);
            }

            builder.ConfigurePipeline(receiverBuilder.GetPipelineName(), receiverBuilder.GetPipeline());

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
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNull(sender, nameof(sender));

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

            builder.ConfigurePipeline(senderBuilder.GetPipelineName(), senderBuilder.GetPipeline());

            return builder;
        }
    }
}