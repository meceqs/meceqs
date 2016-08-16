using System;
using Meceqs;
using Meceqs.Configuration;
using Meceqs.Pipeline;
using Meceqs.Sending;
using Meceqs.Sending.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SendingMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddSender(this IMeceqsBuilder builder, Action<IPipelineBuilder> pipeline, Action<SendOptions> setupAction = null)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(pipeline, nameof(pipeline));

            builder.AddPipeline(SendOptions.DefaultPipelineName, pipeline);

            // Core Services
            builder.Services.TryAddSingleton<IEnvelopeFactory, DefaultEnvelopeFactory>();
            builder.Services.TryAddSingleton<IEnvelopeCorrelator, DefaultEnvelopeCorrelator>();
            builder.Services.TryAddTransient<IMessageSender, MessageSender>();

            if (setupAction != null)
            {
                builder.Services.Configure(setupAction);
            }

            return builder;
        }
    }
}