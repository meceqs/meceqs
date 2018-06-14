using Meceqs.AzureEventHubs.Sending;
using Meceqs.Transport;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.AzureEventHubs.DependencyInjection
{
    public class EventHubSenderBuilder : TransportSenderBuilder<IEventHubSenderBuilder, EventHubSenderOptions>, IEventHubSenderBuilder
    {
        public override IEventHubSenderBuilder Instance => this;

        public EventHubSenderBuilder(IMeceqsBuilder meceqsBuilder, string pipelineName)
            : base(meceqsBuilder, pipelineName)
        {
            ConfigurePipeline(pipeline => pipeline.EndsWith(x => x.RunEventHubSender()));
        }

        public IEventHubSenderBuilder SetConnectionString(string connectionString)
        {
            Guard.NotNullOrWhiteSpace(connectionString, nameof(connectionString));

            return Configure(x => x.EventHubConnectionString = connectionString);
        }
    }
}