using System;
using Meceqs;
using Meceqs.Amqp.Configuration;
using Meceqs.Amqp.Internal;
using Meceqs.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AmqpMeceqsBuilderExtensions
    {
        private static IMeceqsBuilder AddAmqpServices(this IMeceqsBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.TryAddSingleton<IAmqpMessageConverter, DefaultAmqpMessageConverter>();
            builder.Services.TryAddSingleton<ISenderLinkFactory, DefaultSenderLinkFactory>();

            return builder;
        }

        /// <summary>
        /// Adds an AMQP sender pipeline.
        /// </summary>
        public static IMeceqsBuilder AddAmqpSender(this IMeceqsBuilder builder, Action<IAmqpSenderBuilder> sender)
        {
            return AddAmqpSender(builder, null, sender);
        }

        /// <summary>
        /// Adds an AMQP sender pipeline.
        /// </summary>
        public static IMeceqsBuilder AddAmqpSender(
            this IMeceqsBuilder builder,
            IConfiguration configuration,
            Action<IAmqpSenderBuilder> sender)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(sender, nameof(sender));

            if (configuration != null)
            {
                builder.Services.Configure<AmqpSenderOptions>(configuration);
            }

            var senderBuilder = new AmqpSenderBuilder();
            sender?.Invoke(senderBuilder);

            builder.AddAmqpServices();

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