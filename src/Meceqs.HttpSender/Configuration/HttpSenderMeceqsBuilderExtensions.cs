using System;
using Meceqs;
using Meceqs.Configuration;
using Meceqs.HttpSender;
using Meceqs.HttpSender.Configuration;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HttpSenderMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddHttpSender(
            this IMeceqsBuilder builder,
            Action<HttpSenderBuilder> sender)
        {
            return AddHttpSender(builder, null, null, sender);
        }

        public static IMeceqsBuilder AddHttpSender(
                    this IMeceqsBuilder builder,
                    string pipelineName,
                    Action<HttpSenderBuilder> sender)
        {
            return AddHttpSender(builder, pipelineName, null, sender);
        }

        public static IMeceqsBuilder AddHttpSender(
            this IMeceqsBuilder builder,
            IConfiguration configuration,
            Action<HttpSenderBuilder> sender = null)
        {
            return AddHttpSender(builder, null, configuration, sender);
        }

        public static IMeceqsBuilder AddHttpSender(
            this IMeceqsBuilder builder,
            string pipelineName,
            IConfiguration configuration,
            Action<HttpSenderBuilder> sender = null)
        {
            Check.NotNull(builder, nameof(builder));

            if (configuration != null)
            {
                builder.Services.Configure<HttpSenderOptions>(configuration);
            }

            if (sender != null)
            {
                builder.Services.Configure(sender);
            }

            return builder;
        }
    }
}