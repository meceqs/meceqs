using System;
using Meceqs;
using Meceqs.AspNetCore;
using Meceqs.AspNetCore.Configuration;
using Meceqs.AspNetCore.Consuming;
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

            builder.Services.TryAddTransient<IAspNetCoreConsumer, DefaultAspNetCoreConsumer>();

            return builder;
        }

        /// <summary>
        /// Adds an ASP.NET Core consumer pipeline.
        /// </summary>
        public static IMeceqsBuilder AddAspNetCoreConsumer(
            this IMeceqsBuilder builder,
            Action<IAspNetCoreConsumerBuilder> options)
        {
            Check.NotNull(builder, nameof(builder));

            var consumerBuilder = new AspNetCoreConsumerBuilder();
            options?.Invoke(consumerBuilder);

            builder.AddAspNetCore();

            foreach (var assembly in consumerBuilder.GetDeserializationAssemblies())
            {
                builder.AddDeserializationAssembly(assembly);
            }

            var consumerOptions = consumerBuilder.GetConsumerOptions();
            if (consumerOptions != null)
            {
                builder.Services.Configure(consumerOptions);
            }

            builder.AddPipeline(consumerBuilder.GetPipelineName(), consumerBuilder.GetPipeline());

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