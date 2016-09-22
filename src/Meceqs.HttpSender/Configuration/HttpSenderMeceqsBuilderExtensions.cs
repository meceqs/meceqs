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
            Check.NotNull(builder, nameof(builder));

            builder.Services.TryAddSingleton<IHttpClientProvider, DefaultHttpClientProvider>();
            builder.Services.TryAddSingleton<IHttpRequestMessageConverter, DefaultHttpRequestMessageConverter>();

            return builder;
        }

        public static IMeceqsBuilder AddHttpSender(this IMeceqsBuilder builder, Action<IHttpSenderBuilder> sender)
        {
            return AddHttpSender(builder, null, null, sender);
        }

        public static IMeceqsBuilder AddHttpSender(
                    this IMeceqsBuilder builder,
                    string pipelineName,
                    Action<IHttpSenderBuilder> sender)
        {
            return AddHttpSender(builder, pipelineName, null, sender);
        }

        public static IMeceqsBuilder AddHttpSender(
            this IMeceqsBuilder builder,
            IConfiguration configuration,
            Action<IHttpSenderBuilder> sender = null)
        {
            return AddHttpSender(builder, null, configuration, sender);
        }

        public static IMeceqsBuilder AddHttpSender(
            this IMeceqsBuilder builder,
            string pipelineName,
            IConfiguration configuration,
            Action<IHttpSenderBuilder> sender = null)
        {
            Check.NotNull(builder, nameof(builder));

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

            builder.AddSendPipeline(senderBuilder.GetPipelineName(), senderBuilder.GetPipeline());

            return builder;
        }
    }
}