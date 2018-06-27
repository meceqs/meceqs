using Meceqs;
using Meceqs.AzureEventHubs.Sending;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventHubSenderPipelineBuilderExtensions
    {
        public static void RunEventHubSender(this IPipelineBuilder pipeline)
        {
            Guard.NotNull(pipeline, nameof(pipeline));

            pipeline.UseMiddleware<EventHubSenderMiddleware>();
        }
    }
}