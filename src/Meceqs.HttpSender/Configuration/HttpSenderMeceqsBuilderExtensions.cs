using System;
using Meceqs;
using Meceqs.Configuration;
using Meceqs.HttpSender;
using Meceqs.HttpSender.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HttpSenderMeceqsBuilderExtensions
    {
        private static IMeceqsBuilder AddHttpSenderServices(this IMeceqsBuilder builder)
        {
            Guard.NotNull(builder, nameof(builder));

            builder.Services.TryAddSingleton<IHttpClientProvider, DefaultHttpClientProvider>();
            builder.Services.TryAddSingleton<IHttpRequestMessageConverter, DefaultHttpRequestMessageConverter>();

            return builder;
        }

        public static IMeceqsBuilder AddHttpSender(this IMeceqsBuilder builder, Action<IHttpSenderBuilder> sender)
        {
            return AddHttpSender(builder, null, sender);
        }

        public static IMeceqsBuilder AddHttpSender(
            this IMeceqsBuilder builder,
            IConfiguration configuration,
            Action<IHttpSenderBuilder> sender = null)
        {
            Guard.NotNull(builder, nameof(builder));

            if (configuration != null)
            {
                builder.Services.Configure<HttpSenderOptions>(configuration);
            }

            var senderBuilder = new HttpSenderBuilder();
            sender?.Invoke(senderBuilder);

            builder.AddHttpSenderServices();

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