using System;
using Meceqs;
using Meceqs.Pipeline;
using Meceqs.TypedHandling;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TypedHandlingPipelineBuilderExtensions
    {

        public static void RunTypedHandling(this IPipelineBuilder builder, Action<TypedHandlingOptions> options)
        {
            var handlingOptions = new TypedHandlingOptions();
            options?.Invoke(handlingOptions);

            RunTypedHandling(builder, handlingOptions);
        }

        public static void RunTypedHandling(this IPipelineBuilder builder, TypedHandlingOptions options)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(options, nameof(options));

            builder.UseFilter<TypedHandlingFilter>(options);
        }
    }
}