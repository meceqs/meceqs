using Meceqs;
using Meceqs.AzureServiceBus.Filters;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventHubPublisherPipelineBuilderExtensions
    {
        public static IPipelineBuilder RunEventHubPublisher(this IPipelineBuilder pipeline)
        {
            Check.NotNull(pipeline, nameof(pipeline));

            pipeline.UseFilter<EventHubPublisherFilter>();

            return pipeline;
        }
    }
}