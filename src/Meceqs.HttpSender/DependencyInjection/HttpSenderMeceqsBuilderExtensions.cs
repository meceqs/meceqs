using Meceqs;
using Meceqs.HttpSender;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HttpSenderMeceqsBuilderExtensions
    {
        private static void AddHttpSenderServices(this IMeceqsBuilder builder)
        {
            Guard.NotNull(builder, nameof(builder));

            builder.Services.TryAddSingleton<IHttpRequestMessageConverter, DefaultHttpRequestMessageConverter>();
        }

        public static IMeceqsBuilder AddHttpSender(this IMeceqsBuilder builder, Action<HttpSenderBuilder> sender = null)
        {
            return AddHttpSender(builder, null, sender);
        }

        public static IMeceqsBuilder AddHttpSender(this IMeceqsBuilder builder, string pipelineName, Action<HttpSenderBuilder> sender = null)
        {
            Guard.NotNull(builder, nameof(builder));

            builder.AddHttpSenderServices();

            var senderBuilder = new HttpSenderBuilder(builder, pipelineName);
            sender?.Invoke(senderBuilder);

            senderBuilder.Build();

            return builder;
        }
    }
}
