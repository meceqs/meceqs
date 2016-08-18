using System;
using System.Linq;
using System.Reflection;
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

        public static IMeceqsBuilder AddTypedHandler<THandlerImplementation>(this IMeceqsBuilder builder)
            where THandlerImplementation : IHandles
        {
            return AddTypedHandler(builder, typeof(THandlerImplementation));
        }

        public static IMeceqsBuilder AddTypedHandler(this IMeceqsBuilder builder, Type handler)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(handler, nameof(handler));

            Type baseHandle = typeof(IHandles);
            foreach (var handleInterface in handler.GetTypeInfo().ImplementedInterfaces)
            {
                if (baseHandle.IsAssignableFrom(handleInterface))
                {
                    builder.Services.TryAddTransient(handleInterface, handler);
                }
            }

            return builder;
        }

        public static IMeceqsBuilder AddTypedHandlersFromAssembly<TType>(this IMeceqsBuilder builder)
        {
            return AddTypedHandlersFromAssembly(builder, typeof(TType).GetTypeInfo().Assembly);
        }

        public static IMeceqsBuilder AddTypedHandlersFromAssembly(this IMeceqsBuilder builder, Assembly assembly)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(assembly, nameof(assembly));

            var handlers = from type in assembly.GetTypes()
                           where type.GetTypeInfo().IsClass && !type.GetTypeInfo().IsAbstract
                           where typeof(IHandles).IsAssignableFrom(type)
                           select type;

            foreach (var handler in handlers)
            {
                AddTypedHandler(builder, handler);
            }

            return builder;
        }

        public static IMeceqsBuilder AddTypedHandlingInterceptor<THandleInterceptor>(this IMeceqsBuilder builder)
            where THandleInterceptor : class, IHandleInterceptor
        {
            return AddTypedHandlingInterceptor(builder, typeof(THandleInterceptor));
        }

        public static IMeceqsBuilder AddTypedHandlingInterceptor(this IMeceqsBuilder builder, Type interceptor)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(interceptor, nameof(interceptor));

            builder.Services.AddTransient(typeof(IHandleInterceptor), interceptor);

            return builder;
        }
    }
}