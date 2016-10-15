using Meceqs.Transport;

namespace Meceqs.AspNetCore.Configuration
{
    public class AspNetCoreReceiverBuilder : TransportReceiverBuilder<IAspNetCoreReceiverBuilder, AspNetCoreReceiverOptions>,
        IAspNetCoreReceiverBuilder
    {
        public override IAspNetCoreReceiverBuilder Instance => this;
    }
}