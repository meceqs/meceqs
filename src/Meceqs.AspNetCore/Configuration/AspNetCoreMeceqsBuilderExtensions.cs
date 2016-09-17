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
        /// A meta function for adding an ASP.NET Core consumer with one call.
        /// It adds the most common configuration options in <paramref name="options"/>.
        /// </summary>
        public static IMeceqsBuilder AddAspNetCoreConsumer(
            this IMeceqsBuilder builder,
            Action<IAspNetCoreConsumerBuilder> options)
        {
            Check.NotNull(builder, nameof(builder));

            var consumerBuilder = new AspNetCoreConsumerBuilder();
            options?.Invoke(consumerBuilder);

            // Add core services if they don't yet exist.
            builder.AddAspNetCore();

            // Add deserialization assemblies
            foreach (var assembly in consumerBuilder.GetDeserializationAssemblies())
            {
                builder.AddDeserializationAssembly(assembly);
            }

            // Set AspNetCoreConsumer options.
            var consumerOptions = consumerBuilder.GetConsumerOptions();
            if (consumerOptions != null)
            {
                builder.Services.Configure(consumerOptions);
            }

            // Add the pipeline.
            builder.AddConsumer(consumerBuilder.GetPipelineName(), consumerBuilder.GetPipeline());

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