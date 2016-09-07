using System;
using Meceqs;
using Meceqs.Filters.TypedHandling;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TypedHandlingPipelineBuilderExtensions
    {

        public static IPipelineBuilder RunTypedHandling(this IPipelineBuilder builder, Action<TypedHandlingOptions> options)
        {
            var handlingOptions = new TypedHandlingOptions();
            options?.Invoke(handlingOptions);

            return RunTypedHandling(builder, handlingOptions);
        }

        public static IPipelineBuilder RunTypedHandling(this IPipelineBuilder builder, TypedHandlingOptions options)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(options, nameof(options));

            builder.UseFilter<TypedHandlingFilter>(options);

            return builder;
        }
    }
}