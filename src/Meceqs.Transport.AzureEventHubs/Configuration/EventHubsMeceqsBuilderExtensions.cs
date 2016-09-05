using System;
using System.Collections.Generic;
using Meceqs;
using Meceqs.Configuration;
using Meceqs.Filters.TypedHandling;
using Meceqs.Transport.AzureEventHubs.Configuration;
using Meceqs.Transport.AzureEventHubs.Consuming;
using Meceqs.Transport.AzureEventHubs.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventHubsMeceqsBuilderExtensions
    {
        /// <summary>
        /// Adds core services for Azure Event Hubs.
        /// </summary>
        public static IMeceqsBuilder AddEventHubsCore(this IMeceqsBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.TryAddSingleton<IEventDataConverter, DefaultEventDataConverter>();
            builder.Services.TryAddSingleton<IEventHubClientFactory, DefaultEventHubClientFactory>();
            builder.Services.TryAddSingleton<IEventHubConsumer, DefaultEventHubConsumer>();

            return builder;
        }

        #region Consuming

        /// <summary>
        /// A meta function for adding an Azure Event Hubs consumer with one call.
        /// It adds the most common configuration options in <paramref name="options"/>.
        /// </summary>
        public static IMeceqsBuilder AddEventHubConsumer(
            this IMeceqsBuilder builder,
            Action<IEventHubConsumerBuilder> options)
        {
            Check.NotNull(builder, nameof(builder));

            var consumerBuilder = new EventHubConsumerBuilder();
            options?.Invoke(consumerBuilder);

            // Add core services if they don't yet exist.
            builder.AddEventHubsCore();

            // Set EventHubConsumer options.
            var consumerOptions = consumerBuilder.GetConsumerOptions();
            if (consumerOptions != null)
            {
                builder.Services.Configure(consumerOptions);
            }

            // Add assets from typed handlers.
            foreach (var typedHandler in consumerBuilder.GetTypedHandlers())
            {
                builder.AddEventHubConsumerAssetsFromTypedHandler(typedHandler);
            }

            // Add the pipeline.
            builder.AddConsumer(consumerBuilder.GetPipelineName(), consumerBuilder.GetPipeline());

            return builder;
        }

        public static IMeceqsBuilder AddEventHubConsumerAssetsFromTypedHandler<THandler>(this IMeceqsBuilder builder)
            where THandler : IHandles
        {
            return AddEventHubConsumerAssetsFromTypedHandler(builder, typeof(THandler));
        }

        public static IMeceqsBuilder AddEventHubConsumerAssetsFromTypedHandler(this IMeceqsBuilder builder, Type handlerType)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(handlerType, nameof(handlerType));

            if (!typeof(IHandles).IsAssignableFrom(handlerType))
            {
                throw new ArgumentException(
                    $"Type '{handlerType}' must derive from '{typeof(IHandles)}'",
                    nameof(handlerType));
            }

            // Registers the typed handler itself.
            IList<Type> messageTypes;
            builder.AddTypedHandler(handlerType, out messageTypes);

            foreach (var messageType in messageTypes)
            {
                // Ensure the EventHubConsumer accepts the message.
                builder.Services.Configure<EventHubConsumerOptions>(options => options.AddMessageType(messageType));

                // Ensure deserialization for this message type works.
                builder.AddDeserializationAssembly(messageType.Assembly);
            }

            return builder;
        }

        #endregion Consuming
    }
}