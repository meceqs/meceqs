using Meceqs;
using Meceqs.AzureEventHubs.Sending;
using Meceqs.Transport;

namespace Microsoft.Extensions.DependencyInjection
{
    public class EventHubSenderBuilder : SendTransportBuilder<EventHubSenderBuilder, EventHubSenderOptions>
    {
        protected override EventHubSenderBuilder Instance => this;

        public EventHubSenderBuilder(IMeceqsBuilder meceqsBuilder, string pipelineName)
            : base(meceqsBuilder, pipelineName)
        {
            ConfigurePipeline(pipeline => pipeline.EndsWith(x => x.RunEventHubSender()));
        }

        public EventHubSenderBuilder SetConnectionString(string connectionString)
        {
            Guard.NotNullOrWhiteSpace(connectionString, nameof(connectionString));

            return Configure(x => x.EventHubConnectionString = connectionString);
        }
    }
}
