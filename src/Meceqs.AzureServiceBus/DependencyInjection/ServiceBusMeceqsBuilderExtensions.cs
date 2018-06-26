using Meceqs;
using Meceqs.AzureServiceBus.Internal;
using Meceqs.AzureServiceBus.Receiving;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

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
        public static IMeceqsBuilder AddServiceBusReceiver(this IMeceqsBuilder builder, Action<ServiceBusReceiverBuilder> receiver)
        {
            return AddServiceBusReceiver(builder, null, receiver);
        }

        /// <summary>
        /// Adds an Azure Service Bus receiver that sends messages to the named pipeline.
        /// </summary>
        public static IMeceqsBuilder AddServiceBusReceiver(this IMeceqsBuilder builder, string pipelineName, Action<ServiceBusReceiverBuilder> receiver)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNull(receiver, nameof(receiver));

            builder.AddServiceBusServices();

            var receiverBuilder = new ServiceBusReceiverBuilder(builder, pipelineName);
            receiver?.Invoke(receiverBuilder);

            return builder;
        }

        /// <summary>
        /// Adds the default "Send" pipeline with an Azure Service Bus endpoint.
        /// </summary>
        public static IMeceqsBuilder AddServiceBusSender(this IMeceqsBuilder builder, Action<ServiceBusSenderBuilder> sender = null)
        {
            return AddServiceBusSender(builder, null, sender);
        }

        /// <summary>
        /// Adds the named pipeline with an Azure Service Bus endpoint.
        /// </summary>
        public static IMeceqsBuilder AddServiceBusSender(this IMeceqsBuilder builder, string pipelineName, Action<ServiceBusSenderBuilder> sender = null)
        {
            Guard.NotNull(builder, nameof(builder));

            builder.AddServiceBusServices();

            var senderBuilder = new ServiceBusSenderBuilder(builder, pipelineName);
            sender?.Invoke(senderBuilder);

            return builder;
        }
    }
}