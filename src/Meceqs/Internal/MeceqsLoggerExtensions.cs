using Meceqs.Pipeline;

namespace Microsoft.Extensions.Logging
{
    internal static class MeceqsLoggerExtensions
    {
        // TODO @cweiss static func's etc.

        public static void ExecutingPipeline(this ILogger logger, FilterContext context)
        {
            logger.LogDebug(
                "Executing {MessageType} on pipeline {Pipeline}",
                context.MessageType,
                context.PipelineName);
        }

        public static void SkippingMessage(this ILogger logger, FilterContext context)
        {
            logger.LogDebug(
                "Skipping message of type {MessageType}",
                context.MessageType
            );
        }
    }
}