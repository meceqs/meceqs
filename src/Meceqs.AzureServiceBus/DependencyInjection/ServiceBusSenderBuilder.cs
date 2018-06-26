using Meceqs;
using Meceqs.AzureServiceBus.Sending;
using Meceqs.Transport;

namespace Microsoft.Extensions.DependencyInjection
{
    public class ServiceBusSenderBuilder : SendTransportBuilder<ServiceBusSenderBuilder, ServiceBusSenderOptions>
    {
        protected override ServiceBusSenderBuilder Instance => this;

        public ServiceBusSenderBuilder(IMeceqsBuilder meceqsBuilder, string pipelineName)
            : base(meceqsBuilder, pipelineName)
        {
            Pipeline.EndsWith(x => x.RunServiceBusSender());
        }

        public ServiceBusSenderBuilder SetConnectionString(string connectionString)
        {
            Guard.NotNullOrWhiteSpace(connectionString, nameof(connectionString));

            return Configure(x => x.ConnectionString = connectionString);
        }
    }
}