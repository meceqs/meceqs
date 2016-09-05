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
        public static IMeceqsBuilder AddSender(this IMeceqsBuilder builder, Action<IPipelineBuilder> pipeline)
        {
            return AddSender(builder, null, pipeline);
        }

        public static IMeceqsBuilder AddSender(this IMeceqsBuilder builder, string pipelineName, Action<IPipelineBuilder> pipeline)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(pipeline, nameof(pipeline));

            pipelineName = pipelineName ?? MeceqsDefaults.SendPipelineName;

            builder.AddPipeline(pipelineName, pipeline);

            // Core Services
            builder.Services.TryAddSingleton<IEnvelopeFactory, DefaultEnvelopeFactory>();
            builder.Services.TryAddSingleton<IEnvelopeCorrelator, DefaultEnvelopeCorrelator>();
            builder.Services.TryAddTransient<IMessageSender, MessageSender>();

            return builder;
        }
    }
}