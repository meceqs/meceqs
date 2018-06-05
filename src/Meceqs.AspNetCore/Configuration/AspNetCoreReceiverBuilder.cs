using Meceqs.AspNetCore.Receiving;
using Meceqs.Transport;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.AspNetCore.Configuration
{
    public class AspNetCoreReceiverBuilder : TransportReceiverBuilder<IAspNetCoreReceiverBuilder, AspNetCoreReceiverOptions>,
        IAspNetCoreReceiverBuilder
    {
        public override IAspNetCoreReceiverBuilder Instance => this;

        public AspNetCoreReceiverBuilder(IMeceqsBuilder meceqsBuilder, string pipelineName)
            : base(meceqsBuilder, pipelineName)
        {
        }

        public IAspNetCoreReceiverBuilder SetRoutePrefix(PathString path)
        {
            Configure(x => x.RoutePrefix = path);
            return this;
        }
}
}