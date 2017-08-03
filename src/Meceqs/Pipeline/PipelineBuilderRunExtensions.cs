using Meceqs;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PipelineBuilderRunExtensions
    {
        /// <summary>
        /// Adds a terminal middleware to the pipeline.
        /// </summary>
        public static void Run(this IPipelineBuilder builder, MiddlewareDelegate middleware)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNull(middleware, nameof(middleware));

            builder.Use(_ => middleware);
        }
    }
}