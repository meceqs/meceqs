using Meceqs.Transport;
using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore.Configuration
{
    public class AspNetCoreReceiverBuilder : TransportReceiverBuilder<IAspNetCoreReceiverBuilder, AspNetCoreReceiverOptions>,
        IAspNetCoreReceiverBuilder
    {
        public override IAspNetCoreReceiverBuilder Instance => this;

        public IAspNetCoreReceiverBuilder UseRoutePrefix(PathString path)
        {
            ConfigureOptions(x => x.RoutePrefix = path);
            return this;
        }
}
}