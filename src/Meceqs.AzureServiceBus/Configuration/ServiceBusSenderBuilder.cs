using Meceqs.AzureServiceBus.Sending;
using Meceqs.Transport;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.AzureServiceBus.Configuration
{
    public class ServiceBusSenderBuilder : TransportSenderBuilder<IServiceBusSenderBuilder, ServiceBusSenderOptions>, IServiceBusSenderBuilder
    {
        public override IServiceBusSenderBuilder Instance => this;

        public ServiceBusSenderBuilder(IServiceCollection services, string pipelineName)
            : base(services, pipelineName)
        {
        }
    }
}