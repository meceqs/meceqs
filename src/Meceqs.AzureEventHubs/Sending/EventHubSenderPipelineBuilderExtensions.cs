using Meceqs;
using Meceqs.AzureEventHubs.Sending;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventHubSenderPipelineBuilderExtensions
    {
        public static void RunEventHubSender(this IPipelineBuilder pipeline)
        {
            Check.NotNull(pipeline, nameof(pipeline));

            pipeline.UseFilter<EventHubSenderFilter>();
        }
    }
}