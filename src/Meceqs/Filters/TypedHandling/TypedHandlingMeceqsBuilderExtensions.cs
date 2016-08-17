using Meceqs;
using Meceqs.Configuration;
using Meceqs.Filters.TypedHandling;
using Meceqs.Filters.TypedHandling.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TypedHandlingMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddTypedHandling(this IMeceqsBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.TryAddTransient<IHandlerFactory, DefaultHandlerFactory>();

            builder.Services.TryAddSingleton<IHandleContextFactory, DefaultHandleContextFactory>();
            builder.Services.TryAddSingleton<IHandlerFactoryInvoker, DefaultHandlerFactoryInvoker>();
            builder.Services.TryAddSingleton<IHandleMethodResolver, DefaultHandleMethodResolver>();
            builder.Services.TryAddSingleton<IHandlerInvoker, DefaultHandlerInvoker>();

            return builder;
        }

        public static IMeceqsBuilder AddTypedHandlingInterceptor<THandleInterceptor>(this IMeceqsBuilder builder)
            where THandleInterceptor : class, IHandleInterceptor
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.AddTransient<IHandleInterceptor, THandleInterceptor>();

            return builder;
        }
    }
}