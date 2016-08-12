using System;
using Meceqs;
using Meceqs.Configuration;
using Meceqs.Sending;
using Meceqs.Sending.Internal;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SendingMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddSender(this IMeceqsBuilder builder, Action<SendOptions> setupAction)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.AddTransient<IConfigureOptions<SendOptions>, SendOptionsSetup>();
            builder.Services.Configure(setupAction);

            builder.Services.AddSingleton<IEnvelopeFactory, DefaultEnvelopeFactory>();

            builder.Services.AddSingleton<ISendPipeline, SendPipeline>();
            builder.Services.AddTransient<IMessageSender, MessageSender>();

            builder.Services.AddSingleton<IEnvelopeCorrelator, DefaultEnvelopeCorrelator>();

            return builder;
        }
    }
}