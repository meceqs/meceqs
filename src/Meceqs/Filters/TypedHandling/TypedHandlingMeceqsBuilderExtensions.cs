using Meceqs;
using Meceqs.Configuration;
using Meceqs.Filters.TypedHandling.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TypedHandlingMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddTypedHandling(this IMeceqsBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.TryAddSingleton<IHandleContextFactory, DefaultHandleContextFactory>();
            builder.Services.TryAddSingleton<IHandleMethodResolver, DefaultHandleMethodResolver>();
            builder.Services.TryAddSingleton<IHandlerInvoker, DefaultHandlerInvoker>();

            return builder;
        }
    }
}