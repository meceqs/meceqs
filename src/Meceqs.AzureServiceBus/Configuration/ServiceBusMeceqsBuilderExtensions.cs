using System;
using Meceqs;
using Meceqs.AzureServiceBus.Configuration;
using Meceqs.AzureServiceBus.Internal;
using Meceqs.AzureServiceBus.Receiving;
using Meceqs.AzureServiceBus.Sending;
using Meceqs.Configuration;
using Microsoft.Extensions.Configuration;
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
            builder.Services.TryAddSingleton<IServiceBusReceiver, DefaultServiceBusReceiver>();

            return builder;
        }

        /// <summary>
        /// Adds an Azure Service Bus receiver pipeline.
        /// </summary>
        public static IMeceqsBuilder AddServiceBusReceiver(this IMeceqsBuilder builder, Action<IServiceBusReceiverBuilder> receiver)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(receiver, nameof(receiver));

            var receiverBuilder = new ServiceBusReceiverBuilder();
            receiver?.Invoke(receiverBuilder);

            builder.AddServiceBusServices();

            foreach (var assembly in receiverBuilder.GetDeserializationAssemblies())
            {
                builder.AddDeserializationAssembly(assembly);
            }

            var receiverOptions = receiverBuilder.GetReceiverOptions();
            if (receiverOptions != null)
            {
                builder.Services.Configure(receiverOptions);
            }

            builder.AddPipeline(receiverBuilder.GetPipelineName(), receiverBuilder.GetPipeline());

            return builder;
        }

        /// <summary>
        /// Adds an Azure Service Bus sender pipeline.
        /// </summary>
        public static IMeceqsBuilder AddServiceBusSender(this IMeceqsBuilder builder, Action<IServiceBusSenderBuilder> sender)
        {
            return AddServiceBusSender(builder, null, sender);
        }

        /// <summary>
        /// Adds an Azure Service Bus sender pipeline.
        /// </summary>
        public static IMeceqsBuilder AddServiceBusSender(
            this IMeceqsBuilder builder,
            IConfiguration configuration,
            Action<IServiceBusSenderBuilder> sender)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(sender, nameof(sender));

            if (configuration != null)
            {
                builder.Services.Configure<ServiceBusSenderOptions>(configuration);
            }

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