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
            string pipelineName,
            IConfiguration configuration,
            Action<IHttpSenderBuilder> sender)
        {
            Guard.NotNull(builder, nameof(builder));

            pipelineName = pipelineName ?? MeceqsDefaults.SendPipelineName;

            builder.AddHttpSenderServices();

            // Code based options
            var senderBuilder = new HttpSenderBuilder(builder.Services, pipelineName);
            sender?.Invoke(senderBuilder);

            // Add the HttpSenderMiddleware as the last middleware
            senderBuilder.ConfigurePipeline(pipeline => pipeline.RunHttpSender());

            // Configuration based options
            if (configuration != null)
            {
                builder.Services.Configure<HttpSenderOptions>(configuration);
            }

            return builder;
        }
    }
}