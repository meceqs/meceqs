using Meceqs;
using Meceqs.Configuration;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventHubSenderMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddEventHubSender(this IMeceqsBuilder builder, IPipelineBuilder pipeline)
        {
            return AddEventHubSender(builder, MeceqsDefaults.ConsumePipelineName, pipeline);
        }

        public static IMeceqsBuilder AddEventHubSender(this IMeceqsBuilder builder, string pipelineName, IPipelineBuilder pipeline)
        {
            Check.NotNull(builder, nameof(builder));

            

            return builder;
        }
    }

}