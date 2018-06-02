using Meceqs;
using Meceqs.AzureServiceBus.Sending;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceBusSenderPipelineOptionsExtensions
    {
        public static void RunServiceBusSender(this PipelineOptions pipeline)
        {
            Guard.NotNull(pipeline, nameof(pipeline));

            pipeline.UseMiddleware<ServiceBusSenderMiddleware>();
        }
    }
}