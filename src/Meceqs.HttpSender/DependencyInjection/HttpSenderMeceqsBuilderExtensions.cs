using System;
using Meceqs;
using Meceqs.HttpSender;
using Meceqs.HttpSender.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HttpSenderMeceqsBuilderExtensions
    {
        private static void AddHttpSenderServices(this IMeceqsBuilder builder)
        {
            Guard.NotNull(builder, nameof(builder));

            builder.Services.TryAddSingleton<IHttpRequestMessageConverter, DefaultHttpRequestMessageConverter>();
        }

        public static IMeceqsBuilder AddHttpSender(this IMeceqsBuilder builder, Action<IHttpSenderBuilder> sender)
        {
            return AddHttpSender(builder, null, null, sender);
        }

        public static IMeceqsBuilder AddHttpSender(this IMeceqsBuilder builder, IConfiguration configuration, Action<IHttpSenderBuilder> sender = null)
        {
            return AddHttpSender(builder, null, configuration, sender);
        }

        public static IMeceqsBuilder AddHttpSender(this IMeceqsBuilder builder, string pipelineName, Action<IHttpSenderBuilder> sender)
        {
            return AddHttpSender(builder, pipelineName, null, sender);
        }

        public static IMeceqsBuilder AddHttpSender(
            this IMeceqsBuilder builder,
            string pipelineName,
            IConfiguration configuration,
            Action<IHttpSenderBuilder> sender = null)
        {
            Guard.NotNull(builder, nameof(builder));

            builder.AddHttpSenderServices();

            var senderBuilder = new HttpSenderBuilder(builder, pipelineName);

            // Code based options
            sender?.Invoke(senderBuilder);

            // Configuration based options
            if (configuration != null)
            {
                builder.Services.Configure<HttpSenderOptions>(senderBuilder.PipelineName, configuration);
            }

            return builder;
        }
    }
}