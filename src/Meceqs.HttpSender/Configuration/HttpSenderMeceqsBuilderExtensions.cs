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
        public static IMeceqsBuilder AddHttpSenderCore(this IMeceqsBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.TryAddSingleton<IHttpClientProvider, DefaultHttpClientProvider>();
            builder.Services.TryAddSingleton<IHttpRequestMessageConverter, DefaultHttpRequestMessageConverter>();

            return builder;
        }

        public static IMeceqsBuilder AddHttpSender(
            this IMeceqsBuilder builder,
            Action<IHttpSenderBuilder> sender)
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

            // Add core services if they don't yet exist.
            builder.AddHttpSenderCore();

            // Add options.
            var senderOptions = senderBuilder.GetSenderOptions();
            if (senderOptions != null)
            {
                builder.Services.Configure(senderOptions);
            }

            // Add the pipeline.
            builder.AddSender(senderBuilder.GetPipelineName(), senderBuilder.GetPipeline());

            return builder;
        }
    }
}