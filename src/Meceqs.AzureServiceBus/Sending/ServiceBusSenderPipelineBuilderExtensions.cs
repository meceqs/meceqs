using Meceqs;
using Meceqs.AzureServiceBus.Sending;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceBusSenderPipelineBuilderExtensions
    {
        public static IPipelineBuilder RunServiceBusSender(this IPipelineBuilder pipeline)
        {
            Check.NotNull(pipeline, nameof(pipeline));

            pipeline.UseFilter<ServiceBusSenderFilter>();

            return pipeline;
        }
    }
}