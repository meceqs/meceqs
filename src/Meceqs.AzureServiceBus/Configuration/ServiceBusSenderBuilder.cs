using Meceqs.AzureServiceBus.Sending;
using Meceqs.Configuration;
using Meceqs.Transport;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.AzureServiceBus.Configuration
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