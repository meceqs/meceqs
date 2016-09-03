using Meceqs;
using Meceqs.Transport.AzureEventHubs;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventHubSenderPipelineBuilderExtensions
    {
        public static IPipelineBuilder RunEventHubSender(this IPipelineBuilder pipeline)
        {
            Check.NotNull(pipeline, nameof(pipeline));

            pipeline.UseFilter<EventHubSenderFilter>();

            return pipeline;
        }
    }
}