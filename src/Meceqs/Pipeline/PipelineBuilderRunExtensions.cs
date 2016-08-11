using Meceqs;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PipelineBuilderRunExtensions
    {
        /// <summary>
        /// Terminal filter.
        /// </summary>
        public static void Run(this IPipelineBuilder builder, FilterDelegate filter)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(filter, nameof(filter));

            builder.Use(_ => filter);
        }
    }
}