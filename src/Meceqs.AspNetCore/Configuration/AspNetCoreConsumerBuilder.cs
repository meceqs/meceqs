using Meceqs.Transport;

namespace Meceqs.AspNetCore.Configuration
{
    public class AspNetCoreConsumerBuilder : TransportConsumerBuilder<IAspNetCoreConsumerBuilder, AspNetCoreConsumerOptions>,
        IAspNetCoreConsumerBuilder
    {
        public override IAspNetCoreConsumerBuilder Instance => this;
    }
}