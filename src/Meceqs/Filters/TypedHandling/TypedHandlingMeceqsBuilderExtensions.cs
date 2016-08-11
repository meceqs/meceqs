using Meceqs;
using Meceqs.Filters.TypedHandling;
using Meceqs.Filters.TypedHandling.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TypedHandlingMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddTypedHandling(this IMeceqsBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.AddTransient<IHandlerFactory, DefaultHandlerFactory>();

            builder.Services.AddSingleton<IHandleContextFactory, DefaultHandleContextFactory>();
            builder.Services.AddSingleton<IHandlerFactoryInvoker, DefaultHandlerFactoryInvoker>();
            builder.Services.AddSingleton<IHandleMethodResolver, DefaultHandleMethodResolver>();
            builder.Services.AddSingleton<IHandlerInvoker, DefaultHandlerInvoker>();

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