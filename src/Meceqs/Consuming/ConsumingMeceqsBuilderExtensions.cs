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
        public static IMeceqsBuilder AddConsumer(this IMeceqsBuilder builder, Action<IPipelineBuilder> pipeline, Action<ConsumeOptions> setupAction = null)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(pipeline, nameof(pipeline));

            builder.AddPipeline(ConsumeOptions.DefaultPipelineName, pipeline);

            // Core Services
            builder.Services.AddTransient<IMessageConsumer, MessageConsumer>();

            if (setupAction != null)
            {
                builder.Services.Configure(setupAction);
            }

            return builder;
        }
    }
}