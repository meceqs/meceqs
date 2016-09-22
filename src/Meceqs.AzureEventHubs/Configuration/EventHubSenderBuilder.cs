using Meceqs.AzureEventHubs.Sending;
using Meceqs.Transport;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.AzureEventHubs.Configuration
{
    public class EventHubSenderBuilder : TransportSenderBuilder<IEventHubSenderBuilder, EventHubSenderOptions>, IEventHubSenderBuilder
    {
        public override IEventHubSenderBuilder Instance => this;

        public EventHubSenderBuilder()
        {
            PipelineEndHook = pipeline => pipeline.RunEventHubSender();
        }
    }
}