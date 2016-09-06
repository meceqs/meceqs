using Meceqs;
using Meceqs.Pipeline;
using Meceqs.Transport.AzureServiceBus.Sending;

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