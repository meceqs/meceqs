using Meceqs.AspNetCore.Receiving;
using Meceqs.Transport;
using Microsoft.AspNetCore.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public class AspNetCoreReceiverBuilder : ReceiveTransportBuilder<AspNetCoreReceiverBuilder, AspNetCoreReceiverOptions>
    {
        protected override AspNetCoreReceiverBuilder Instance => this;

        public AspNetCoreReceiverBuilder(IMeceqsBuilder meceqsBuilder, string pipelineName)
            : base(meceqsBuilder, pipelineName)
        {
        }

        public AspNetCoreReceiverBuilder SetRoutePrefix(PathString path)
        {
            return Configure(x => x.RoutePrefix = path);
        }
}
}
