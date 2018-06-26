using Meceqs;
using Meceqs.AzureEventHubs.Internal;
using Meceqs.AzureEventHubs.Receiving;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

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
        public static IMeceqsBuilder AddEventHubReceiver(this IMeceqsBuilder builder, Action<EventHubReceiverBuilder> receiver)
        {
            return AddEventHubReceiver(builder, null, receiver);
        }

        /// <summary>
        /// Adds an Azure Event Hubs receiver that sends messages to the named pipeline.
        /// </summary>
        public static IMeceqsBuilder AddEventHubReceiver(this IMeceqsBuilder builder, string pipelineName, Action<EventHubReceiverBuilder> receiver)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNull(receiver, nameof(receiver));

            builder.AddEventHubServices();

            var receiverBuilder = new EventHubReceiverBuilder(builder, pipelineName);
            receiver?.Invoke(receiverBuilder);

            return builder;
        }

        /// <summary>
        /// Adds the default "Send" pipeline with an Azure Event Hubs endpoint.
        /// </summary>
        public static IMeceqsBuilder AddEventHubSender(this IMeceqsBuilder builder, Action<EventHubSenderBuilder> sender = null)
        {
            return AddEventHubSender(builder, null, sender);
        }

        /// <summary>
        /// Adds the named pipeline with an Azure Event Hubs endpoint.
        /// </summary>
        public static IMeceqsBuilder AddEventHubSender(this IMeceqsBuilder builder, string pipelineName, Action<EventHubSenderBuilder> sender = null)
        {
            Guard.NotNull(builder, nameof(builder));

            builder.AddEventHubServices();

            var senderBuilder = new EventHubSenderBuilder(builder, pipelineName);
            sender?.Invoke(senderBuilder);

            return builder;
        }
    }
}