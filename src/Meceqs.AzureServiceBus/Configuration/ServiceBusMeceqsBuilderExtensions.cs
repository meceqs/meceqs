using System;
using Meceqs;
using Meceqs.AzureServiceBus.Configuration;
using Meceqs.AzureServiceBus.Consuming;
using Meceqs.AzureServiceBus.Internal;
using Meceqs.Configuration;
using Meceqs.Pipeline;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceBusMeceqsBuilderExtensions
    {
        private static IMeceqsBuilder AddServiceBusServices(this IMeceqsBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.TryAddSingleton<IBrokeredMessageConverter, DefaultBrokeredMessageConverter>();
            builder.Services.TryAddSingleton<IBrokeredMessageInvoker, DefaultBrokeredMessageInvoker>();
            builder.Services.TryAddSingleton<IServiceBusMessageSenderFactory, DefaultServiceBusMessageSenderFactory>();
            builder.Services.TryAddSingleton<IServiceBusConsumer, DefaultServiceBusConsumer>();

            return builder;
        }

        #region Consuming

        /// <summary>
        /// A meta function for adding an Azure Service Bus consumer with one call.
        /// It adds the most common configuration options in <paramref name="consumer"/>.
        /// </summary>
        public static IMeceqsBuilder AddServiceBusConsumer(
            this IMeceqsBuilder builder,
            Action<IServiceBusConsumerBuilder> consumer)
        {
            Check.NotNull(builder, nameof(builder));

            var consumerBuilder = new ServiceBusConsumerBuilder();
            consumer?.Invoke(consumerBuilder);

            // Add core services if they don't yet exist.
            builder.AddServiceBusServices();

            // Add deserialization assemblies
            foreach (var assembly in consumerBuilder.GetDeserializationAssemblies())
            {
                builder.AddDeserializationAssembly(assembly);
            }

            // Set ServiceBus options.
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

        public static IMeceqsBuilder AddServiceBusSender(this IMeceqsBuilder builder, Action<IPipelineBuilder> pipeline)
        {
            return AddServiceBusSender(builder, null, pipeline);
        }

        public static IMeceqsBuilder AddServiceBusSender(this IMeceqsBuilder builder, string pipelineName, Action<IPipelineBuilder> pipeline)
        {
            Check.NotNull(builder, nameof(builder));

            builder.AddServiceBusServices();

            builder.AddSender(pipelineName, pipeline);

            return builder;
        }

        #endregion
    }


}