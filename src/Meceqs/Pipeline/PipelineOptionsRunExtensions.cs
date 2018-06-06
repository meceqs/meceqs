using Meceqs;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PipelineOptionsRunExtensions
    {
        /// <summary>
        /// Adds a terminal middleware to the pipeline.
        /// </summary>
        public static void Run(this PipelineOptions builder, MiddlewareDelegate middleware)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNull(middleware, nameof(middleware));

            builder.Use((_, __) => middleware);
        }
    }
}