using System;
using Meceqs;
using Meceqs.AzureServiceBus.Consuming;
using Meceqs.AzureServiceBus.Internal;
using Meceqs.Configuration;
using Meceqs.Pipeline;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceBusMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddServiceBusCore(this IMeceqsBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.TryAddSingleton<IBrokeredMessageConverter, DefaultBrokeredMessageConverter>();
            builder.Services.TryAddSingleton<IBrokeredMessageInvoker, DefaultBrokeredMessageInvoker>();
            builder.Services.TryAddSingleton<IServiceBusMessageSenderFactory, DefaultServiceBusMessageSenderFactory>();
            builder.Services.TryAddSingleton<IServiceBusConsumer, DefaultServiceBusConsumer>();

            return builder;
        }

        #region Sending

        public static IMeceqsBuilder AddServiceBusSender(this IMeceqsBuilder builder, Action<IPipelineBuilder> pipeline)
        {
            return AddServiceBusSender(builder, null, pipeline);
        }

        public static IMeceqsBuilder AddServiceBusSender(this IMeceqsBuilder builder, string pipelineName, Action<IPipelineBuilder> pipeline)
        {
            Check.NotNull(builder, nameof(builder));

            builder.AddServiceBusCore();

            builder.AddSender(pipelineName, pipeline);

            return builder;
        }

        #endregion
    }


}