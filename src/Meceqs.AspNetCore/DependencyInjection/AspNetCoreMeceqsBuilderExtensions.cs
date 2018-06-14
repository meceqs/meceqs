using System;
using Meceqs;
using Meceqs.AspNetCore;
using Meceqs.AspNetCore.DependencyInjection;
using Meceqs.AspNetCore.Receiving;
using Meceqs.Pipeline;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AspNetCoreMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddAspNetCore(this IMeceqsBuilder builder)
        {
            Guard.NotNull(builder, nameof(builder));

            builder.Services.AddHttpContextAccessor();

            // Enricher
            builder.Services.TryAddSingleton<IMessageContextEnricher, AspNetCoreEnricher>();

            // TODO should some be singleton?
            builder.Services.TryAddSingleton<IMessagePathConvention, DefaultMessagePathConvention>();
            builder.Services.TryAddTransient<IHttpRequestReader, DefaultHttpRequestReader>();
            builder.Services.TryAddTransient<IHttpResponseWriter, DefaultHttpResponseWriter>();

            builder.Services.TryAddTransient<IAspNetCoreReceiver, DefaultAspNetCoreReceiver>();

            return builder;
        }

        /// <summary>
        /// Adds an ASP.NET Core receiver that sends messages to the default <see cref="MeceqsDefaults.ReceivePipelineName"/> pipeline.
        /// </summary>
        public static IMeceqsBuilder AddAspNetCoreReceiver(
            this IMeceqsBuilder builder,
            Action<IAspNetCoreReceiverBuilder> receiver)
        {
            return AddAspNetCoreReceiver(builder, null, receiver);
        }

        /// <summary>
        /// Adds an ASP.NET Core receiver that sends messages to the named pipeline.
        /// </summary>
        public static IMeceqsBuilder AddAspNetCoreReceiver(
            this IMeceqsBuilder builder,
            string pipelineName,
            Action<IAspNetCoreReceiverBuilder> receiver)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNull(receiver, nameof(receiver));

            builder.AddAspNetCore();

            var receiverBuilder = new AspNetCoreReceiverBuilder(builder, pipelineName);

            receiver?.Invoke(receiverBuilder);

            // Register the receiver with the transport
            builder.Services.Configure<ReceiveTransportOptions>(o => o.AddReceiver(receiverBuilder.PipelineName));

            return builder;
        }

        public static IMeceqsBuilder ConfigureAspNetCoreEnricher(
            this IMeceqsBuilder builder,
            Action<AspNetCoreEnricherOptions> options)
        {
            Guard.NotNull(builder, nameof(builder));

            if (options != null)
            {
                builder.Services.Configure(options);
            }

            return builder;
        }
    }
}