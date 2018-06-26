using System;
using Meceqs;
using Meceqs.Pipeline;
using Meceqs.TypedHandling;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TypedHandlingPipelineOptionsExtensions
    {
        public static void RunTypedHandling(this PipelineBuilder pipeline, Action<TypedHandlingOptions> options)
        {
            var handlingOptions = new TypedHandlingOptions();
            options?.Invoke(handlingOptions);

            RunTypedHandling(pipeline, handlingOptions);
        }

        public static void RunTypedHandling(this PipelineBuilder pipeline, TypedHandlingOptions options)
        {
            Guard.NotNull(pipeline, nameof(pipeline));
            Guard.NotNull(options, nameof(options));

            pipeline.UseMiddleware<TypedHandlingMiddleware>(options);
        }
    }
}