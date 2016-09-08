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

        public static IMeceqsBuilder AddAspNetCoreConsumer(this IMeceqsBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            // TODO should some be singleton?
            builder.Services.TryAddSingleton<IMessagePathConvention, DefaultMessagePathConvention>();
            builder.Services.TryAddTransient<IHttpRequestReader, DefaultHttpRequestReader>();
            builder.Services.TryAddTransient<IHttpResponseWriter, DefaultHttpResponseWriter>();

            builder.Services.TryAddTransient<IAspNetCoreConsumer, DefaultAspNetCoreConsumer>();

            return builder;
        }
    }
}