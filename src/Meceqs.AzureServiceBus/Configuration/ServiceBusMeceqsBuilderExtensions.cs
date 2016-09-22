using System;
using Meceqs;
using Meceqs.AzureServiceBus.Configuration;
using Meceqs.AzureServiceBus.Consuming;
using Meceqs.AzureServiceBus.Internal;
using Meceqs.Configuration;
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

        /// <summary>
        /// Adds an Azure Service Bus consumer pipeline.
        /// </summary>
        public static IMeceqsBuilder AddServiceBusConsumer(this IMeceqsBuilder builder, Action<IServiceBusConsumerBuilder> consumer)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(consumer, nameof(consumer));

            var consumerBuilder = new ServiceBusConsumerBuilder();
            consumer?.Invoke(consumerBuilder);

            builder.AddServiceBusServices();

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
        /// Adds an Azure Service Bus sender pipeline.
        /// </summary>
        public static IMeceqsBuilder AddServiceBusSender(this IMeceqsBuilder builder, Action<IServiceBusSenderBuilder> sender)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(sender, nameof(sender));

            var senderBuilder = new ServiceBusSenderBuilder();
            sender?.Invoke(senderBuilder);

            builder.AddServiceBusServices();

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