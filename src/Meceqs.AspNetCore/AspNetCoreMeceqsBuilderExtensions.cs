using System;
using Meceqs;
using Meceqs.AspNetCore.Consuming;
using Meceqs.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AspNetCoreMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddAspNetCore(this IMeceqsBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return builder;
        }

        #region Consuming

        public static IMeceqsBuilder AddAspNetCoreConsumer(this IMeceqsBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.AddAspNetCore();

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
            builder.AddAspNetCoreConsumer();

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

        #endregion Consuming
    }
}