using Meceqs.AspNetCore.Receiving;
using Meceqs.Transport;
using Microsoft.AspNetCore.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface IAspNetCoreReceiverBuilder : ITransportReceiverBuilder<IAspNetCoreReceiverBuilder, AspNetCoreReceiverOptions>
    {
        IAspNetCoreReceiverBuilder SetRoutePrefix(PathString path);
    }
}