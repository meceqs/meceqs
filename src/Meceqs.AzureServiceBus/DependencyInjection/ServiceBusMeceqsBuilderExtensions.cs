using System;
using Meceqs;
using Meceqs.AzureServiceBus.DependencyInjection;
using Meceqs.AzureServiceBus.Internal;
using Meceqs.AzureServiceBus.Receiving;
using Meceqs.AzureServiceBus.Sending;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceBusMeceqsBuilderExtensions
    {
        private static void AddServiceBusServices(this IMeceqsBuilder builder)
        {
            Guard.NotNull(builder, nameof(builder));

            builder.Services.TryAddSingleton<IServiceBusMessageConverter, DefaultServiceBusMessageConverter>();
            builder.Services.TryAddSingleton<IServiceBusMessageSenderFactory, DefaultServiceBusMessageSenderFactory>();
            builder.Services.TryAddSingleton<IServiceBusReceiver, DefaultServiceBusReceiver>();
        }

        /// <summary>
        /// Adds an Azure Service Bus receiver that sends messages to the default <see cref="MeceqsDefaults.ReceivePipelineName"/> pipeline.
        /// </summary>
        public static IMeceqsBuilder AddServiceBusReceiver(this IMeceqsBuilder builder, Action<IServiceBusReceiverBuilder> receiver)
        {
            return AddServiceBusReceiver(builder, null, receiver);
        }

        /// <summary>
        /// Adds an Azure Service Bus receiver that sends messages to the named pipeline.
        /// </summary>
        public static IMeceqsBuilder AddServiceBusReceiver(this IMeceqsBuilder builder, string pipelineName, Action<IServiceBusReceiverBuilder> receiver)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNull(receiver, nameof(receiver));

            builder.AddServiceBusServices();

            var receiverBuilder = new ServiceBusReceiverBuilder(builder, pipelineName);
            receiver?.Invoke(receiverBuilder);

            return builder;
        }

        /// <summary>
        /// Adds an Azure Service Bus sender to the default "Send" pipeline.
        /// </summary>
        public static IMeceqsBuilder AddServiceBusSender(this IMeceqsBuilder builder, Action<IServiceBusSenderBuilder> sender)
        {
            return AddServiceBusSender(builder, null, sender);
        }

        /// <summary>
        /// Adds an Azure Service Bus sender to the given pipeline.
        /// </summary>
        public static IMeceqsBuilder AddServiceBusSender(this IMeceqsBuilder builder, string pipelineName, Action<IServiceBusSenderBuilder> sender)
        {
            return AddServiceBusSender(builder, pipelineName, null, sender);
        }

        /// <summary>
        /// Adds an Azure Service Bus sender to the given pipeline.
        /// </summary>
        public static IMeceqsBuilder AddServiceBusSender(
            this IMeceqsBuilder builder,
            string pipelineName,
            IConfiguration configuration,
            Action<IServiceBusSenderBuilder> sender)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNull(sender, nameof(sender));

            builder.AddServiceBusServices();

            // Code based options
            var senderBuilder = new ServiceBusSenderBuilder(builder, pipelineName);
            sender?.Invoke(senderBuilder);

            // Configuration based options
            if (configuration != null)
            {
                builder.Services.Configure<ServiceBusSenderOptions>(senderBuilder.PipelineName, configuration);
            }

            return builder;
        }
    }
}