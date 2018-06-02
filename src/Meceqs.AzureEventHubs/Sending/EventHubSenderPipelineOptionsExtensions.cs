using Meceqs;
using Meceqs.AzureEventHubs.Sending;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventHubSenderPipelineOptionsExtensions
    {
        public static void RunEventHubSender(this PipelineOptions pipeline)
        {
            Guard.NotNull(pipeline, nameof(pipeline));

            pipeline.UseMiddleware<EventHubSenderMiddleware>();
        }
    }
}