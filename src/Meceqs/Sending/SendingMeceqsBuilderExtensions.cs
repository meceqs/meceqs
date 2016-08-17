using System;
using Meceqs;
using Meceqs.Configuration;
using Meceqs.Pipeline;
using Meceqs.Sending;
using Meceqs.Sending.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SendingMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddSender(this IMeceqsBuilder builder, Action<IPipelineBuilder> pipeline)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(pipeline, nameof(pipeline));

            builder.AddPipeline(SendOptions.DefaultPipelineName, pipeline);

            builder.Services.AddSingleton<IEnvelopeFactory, DefaultEnvelopeFactory>();
            builder.Services.AddSingleton<IEnvelopeCorrelator, DefaultEnvelopeCorrelator>();
            builder.Services.AddTransient<IMessageSender, MessageSender>();

            return builder;
        }
    }
}