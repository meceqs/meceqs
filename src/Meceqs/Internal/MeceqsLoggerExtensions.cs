using Meceqs.Pipeline;

namespace Microsoft.Extensions.Logging
{
    internal static class MeceqsLoggerExtensions
    {
        public static void ExecutingPipeline(this ILogger logger, FilterContext context)
        {
            logger.LogDebug(
                "Executing {MessageType} on pipeline {Pipeline}",
                context.MessageType,
                context.PipelineName);
        }
    }
}