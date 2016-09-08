using System;
using Meceqs;
using Meceqs.AzureEventHubs.Configuration;
using Meceqs.AzureEventHubs.Consuming;
using Meceqs.AzureEventHubs.Internal;
using Meceqs.Configuration;
using Meceqs.Pipeline;
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

            // Add deserialization assemblies
            foreach (var assembly in consumerBuilder.GetDeserializationAssemblies())
            {
                builder.AddDeserializationAssembly(assembly);
            }

            // Set EventHubConsumer options.
            var consumerOptions = consumerBuilder.GetConsumerOptions();
            if (consumerOptions != null)
            {
                builder.Services.Configure(consumerOptions);
            }

            // Add the pipeline.
            builder.AddConsumer(consumerBuilder.GetPipelineName(), consumerBuilder.GetPipeline());

            return builder;
        }

        #endregion Consuming

        #region Sending

        public static IMeceqsBuilder AddEventHubSender(this IMeceqsBuilder builder, Action<IPipelineBuilder> pipeline)
        {
            return AddEventHubSender(builder, null, pipeline);
        }

        public static IMeceqsBuilder AddEventHubSender(this IMeceqsBuilder builder, string pipelineName, Action<IPipelineBuilder> pipeline)
        {
            Check.NotNull(builder, nameof(builder));

            builder.AddEventHubsCore();

            builder.AddSender(pipelineName, pipeline);

            return builder;
        }

        #endregion
    }
}