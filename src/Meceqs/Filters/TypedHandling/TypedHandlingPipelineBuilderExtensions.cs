using System;
using Meceqs;
using Meceqs.Filters.TypedHandling;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TypedHandlingPipelineBuilderExtensions
    {
        public static IPipelineBuilder UseTypedHandling(this IPipelineBuilder builder, Action<TypedHandlingOptions> options = null)
        {
            Check.NotNull(builder, nameof(builder));

            var handlingOptions = new TypedHandlingOptions();
            options?.Invoke(handlingOptions);

            builder.UseFilter<TypedHandlingFilter>(handlingOptions);

            return builder;
        }
    }
}