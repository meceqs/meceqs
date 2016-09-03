using System;
using Meceqs;
using Meceqs.Configuration;
using Meceqs.Consuming;
using Meceqs.Consuming.Internal;
using Meceqs.Pipeline;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConsumingMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddConsumer(this IMeceqsBuilder builder, Action<IPipelineBuilder> pipeline)
        {
            return AddConsumer(builder, MeceqsDefaults.ConsumePipelineName, pipeline);
        }

        public static IMeceqsBuilder AddConsumer(this IMeceqsBuilder builder, string pipelineName, Action<IPipelineBuilder> pipeline)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));
            Check.NotNull(pipeline, nameof(pipeline));

            builder.AddPipeline(pipelineName, pipeline);

            // Core Services
            builder.Services.TryAddTransient<IMessageConsumer, MessageConsumer>();

            return builder;
        }
    }
}