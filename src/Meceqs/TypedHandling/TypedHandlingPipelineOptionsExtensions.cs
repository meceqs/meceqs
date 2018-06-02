using System;
using Meceqs;
using Meceqs.Pipeline;
using Meceqs.TypedHandling;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TypedHandlingPipelineOptionsExtensions
    {
        public static void RunTypedHandling(this PipelineOptions pipeline, Action<TypedHandlingOptions> options)
        {
            var handlingOptions = new TypedHandlingOptions();
            options?.Invoke(handlingOptions);

            RunTypedHandling(pipeline, handlingOptions);
        }

        public static void RunTypedHandling(this PipelineOptions pipeline, TypedHandlingOptions options)
        {
            Guard.NotNull(pipeline, nameof(pipeline));
            Guard.NotNull(options, nameof(options));

            pipeline.UseMiddleware<TypedHandlingMiddleware>(options);
        }
    }
}