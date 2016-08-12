using System;
using Meceqs;
using Meceqs.Configuration;
using Meceqs.Consuming;
using Meceqs.Consuming.Internal;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConsumingMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddConsumer(this IMeceqsBuilder builder, Action<ConsumeOptions> setupAction)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.AddTransient<IConfigureOptions<ConsumeOptions>, ConsumeOptionsSetup>();
            builder.Services.Configure(setupAction);

            builder.Services.AddSingleton<IConsumePipeline, ConsumePipeline>();
            builder.Services.AddTransient<IMessageConsumer, MessageConsumer>();

            return builder;
        }
    }
}