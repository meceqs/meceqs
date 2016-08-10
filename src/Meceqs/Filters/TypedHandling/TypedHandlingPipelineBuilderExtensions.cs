using Meceqs;
using Meceqs.Filters.TypedHandling;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TypedHandlingPipelineBuilderExtensions
    {
        public static IPipelineBuilder UseTypedHandling(this IPipelineBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.UseFilter<TypedHandlingFilter>();

            return builder;
        }
    }
}