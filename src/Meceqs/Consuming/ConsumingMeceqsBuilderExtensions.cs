using System;
using Meceqs;
using Meceqs.Configuration;
using Meceqs.Consuming;
using Meceqs.Consuming.Internal;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConsumingMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddConsumer(this IMeceqsBuilder builder, Action<IPipelineBuilder> pipeline)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(pipeline, nameof(pipeline));

            builder.AddPipeline(ConsumeOptions.DefaultPipelineName, pipeline);

            builder.Services.AddTransient<IMessageConsumer, MessageConsumer>();

            return builder;
        }
    }
}