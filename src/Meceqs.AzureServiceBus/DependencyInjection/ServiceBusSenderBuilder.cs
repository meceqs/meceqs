using Meceqs.AzureServiceBus.Sending;
using Meceqs.Transport;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.AzureServiceBus.DependencyInjection
{
    public class ServiceBusSenderBuilder : TransportSenderBuilder<IServiceBusSenderBuilder, ServiceBusSenderOptions>, IServiceBusSenderBuilder
    {
        public override IServiceBusSenderBuilder Instance => this;

        public ServiceBusSenderBuilder(IMeceqsBuilder meceqsBuilder, string pipelineName)
            : base(meceqsBuilder, pipelineName)
        {
            ConfigurePipeline(pipeline => pipeline.EndsWith(x => x.RunServiceBusSender()));
        }

        public IServiceBusSenderBuilder SetConnectionString(string connectionString)
        {
            Guard.NotNullOrWhiteSpace(connectionString, nameof(connectionString));

            return Configure(x => x.ConnectionString = connectionString);
        }
    }
}