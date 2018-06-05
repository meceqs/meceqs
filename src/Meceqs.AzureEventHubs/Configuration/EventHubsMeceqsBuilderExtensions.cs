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
        private static void AddEventHubServices(this IMeceqsBuilder builder)
        {
            Guard.NotNull(builder, nameof(builder));

            builder.Services.TryAddSingleton<IEventDataConverter, DefaultEventDataConverter>();
            builder.Services.TryAddSingleton<IEventHubClientFactory, DefaultEventHubClientFactory>();
            builder.Services.TryAddSingleton<IEventHubReceiver, DefaultEventHubReceiver>();
        }

        /// <summary>
        /// Adds an Azure Event Hubs receiver that sends messages to the default <see cref="MeceqsDefaults.ReceivePipelineName"/> pipeline.
        /// </summary>
        public static IMeceqsBuilder AddEventHubReceiver(this IMeceqsBuilder builder, Action<IEventHubReceiverBuilder> receiver)
        {
            return AddEventHubReceiver(builder, null, receiver);
        }

        /// <summary>
        /// Adds an Azure Event Hubs receiver that sends messages to the named pipeline.
        /// </summary>
        public static IMeceqsBuilder AddEventHubReceiver(this IMeceqsBuilder builder, string pipelineName, Action<IEventHubReceiverBuilder> receiver)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNull(receiver, nameof(receiver));

            builder.AddEventHubServices();

            var receiverBuilder = new EventHubReceiverBuilder(builder, pipelineName);
            receiver?.Invoke(receiverBuilder);

            return builder;
        }

        /// <summary>
        /// Adds an Azure Event Hubs sender to the default "Send" pipeline.
        /// </summary>
        public static IMeceqsBuilder AddEventHubSender(this IMeceqsBuilder builder, Action<IEventHubSenderBuilder> sender)
        {
            return AddEventHubSender(builder, null, sender);
        }

        /// <summary>
        /// Adds an Azure Event Hubs sender to the given pipeline.
        /// </summary>
        public static IMeceqsBuilder AddEventHubSender(this IMeceqsBuilder builder, string pipelineName, Action<IEventHubSenderBuilder> sender)
        {
            return AddEventHubSender(builder, pipelineName, null, sender);
        }

        /// <summary>
        /// Adds an Azure Event Hubs sender to the given pipeline.
        /// </summary>
        public static IMeceqsBuilder AddEventHubSender(
            this IMeceqsBuilder builder,
            string pipelineName,
            IConfiguration configuration,
            Action<IEventHubSenderBuilder> sender)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNull(sender, nameof(sender));

            builder.AddEventHubServices();

            var senderBuilder = new EventHubSenderBuilder(builder, pipelineName);

            // Code based options
            sender?.Invoke(senderBuilder);

            // Configuration based options
            if (configuration != null)
            {
                builder.Services.Configure<EventHubSenderOptions>(senderBuilder.PipelineName, configuration);
            }

            return builder;
        }
    }
}