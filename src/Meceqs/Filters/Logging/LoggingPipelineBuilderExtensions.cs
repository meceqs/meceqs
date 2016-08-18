using Meceqs;
using Meceqs.Filters.Logging;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LoggingPipelineBuilderExtensions
    {
        public static IPipelineBuilder UseLogging(this IPipelineBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.UseFilter<LoggingFilter>();

            return builder;
        }
    }
}