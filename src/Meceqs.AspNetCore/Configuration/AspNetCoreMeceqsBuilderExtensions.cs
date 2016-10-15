using System;
using Meceqs;
using Meceqs.AspNetCore;
using Meceqs.AspNetCore.Configuration;
using Meceqs.AspNetCore.Receiving;
using Meceqs.Configuration;
using Meceqs.Pipeline;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AspNetCoreMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddAspNetCore(this IMeceqsBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            // Enricher
            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.TryAddSingleton<IFilterContextEnricher, AspNetCoreEnricher>();

            // TODO should some be singleton?
            builder.Services.TryAddSingleton<IMessagePathConvention, DefaultMessagePathConvention>();
            builder.Services.TryAddTransient<IHttpRequestReader, DefaultHttpRequestReader>();
            builder.Services.TryAddTransient<IHttpResponseWriter, DefaultHttpResponseWriter>();

            builder.Services.TryAddTransient<IAspNetCoreReceiver, DefaultAspNetCoreReceiver>();

            return builder;
        }

        /// <summary>
        /// Adds an ASP.NET Core receiver pipeline.
        /// </summary>
        public static IMeceqsBuilder AddAspNetCoreReceiver(
            this IMeceqsBuilder builder,
            Action<IAspNetCoreReceiverBuilder> options)
        {
            Check.NotNull(builder, nameof(builder));

            var receiverBuilder = new AspNetCoreReceiverBuilder();
            options?.Invoke(receiverBuilder);

            builder.AddAspNetCore();

            foreach (var assembly in receiverBuilder.GetDeserializationAssemblies())
            {
                builder.AddDeserializationAssembly(assembly);
            }

            var receiverOptions = receiverBuilder.GetReceiverOptions();
            if (receiverOptions != null)
            {
                builder.Services.Configure(receiverOptions);
            }

            builder.AddPipeline(receiverBuilder.GetPipelineName(), receiverBuilder.GetPipeline());

            return builder;
        }

        public static IMeceqsBuilder ConfigureAspNetCoreEnricher(
            this IMeceqsBuilder builder,
            Action<AspNetCoreEnricherOptions> options)
        {
            Check.NotNull(builder, nameof(builder));

            if (options != null)
            {
                builder.Services.Configure(options);
            }

            return builder;
        }
    }
}