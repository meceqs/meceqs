using System;
using System.Collections.Generic;
using Meceqs;
using Meceqs.Configuration;
using Meceqs.Filters.TypedHandling;
using Meceqs.Transport.AzureEventHubs.Consuming;
using Meceqs.Transport.AzureEventHubs.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AzureEventHubsMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddAzureEventHubs(this IMeceqsBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.TryAddSingleton<IEventDataConverter, DefaultEventDataConverter>();
            builder.Services.TryAddSingleton<IEventHubClientFactory, DefaultEventHubClientFactory>();
            builder.Services.TryAddSingleton<IEventHubConsumer, DefaultEventHubConsumer>();

            return builder;
        }

        public static IMeceqsBuilder AddAzureEventHubConsumer(this IMeceqsBuilder builder, Action<EventHubConsumerOptions> options = null)
        {
            Check.NotNull(builder, nameof(builder));

            AddAzureEventHubs(builder);

            if (options != null)
            {
                builder.Services.Configure(options);
            }

            return builder;
        }

        public static IMeceqsBuilder AddAzureEventHubConsumerForTypedHandler<THandler>(this IMeceqsBuilder builder)
            where THandler : IHandles
        {
            return AddAzureEventHubConsumerForTypedHandler(builder, typeof(THandler));
        }

        public static IMeceqsBuilder AddAzureEventHubConsumerForTypedHandler(this IMeceqsBuilder builder, Type handlerType)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(handlerType, nameof(handlerType));

            if (!typeof(IHandles).IsAssignableFrom(handlerType))
            {
                throw new ArgumentException(
                    $"Type '{handlerType}' must derive from '{typeof(IHandles)}'",
                    nameof(handlerType));
            }

            IList<Type> messageTypes;

            // Registers the typed handler itself.
            builder.AddTypedHandler(handlerType, out messageTypes);

            foreach (var messageType in messageTypes)
            {
                // Ensure the EventHubConsumer accepts the message.
                builder.Services.Configure<EventHubConsumerOptions>(options => options.AddMessageType(messageType));

                // TODO should this happen in AddTypedHandler()?

                // Ensure deserialization for this message type works.
                builder.AddDeserializationAssembly(messageType.Assembly);
            }

            return builder;
        }
    }
}