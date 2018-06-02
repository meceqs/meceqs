using Meceqs.Transport;
using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore.Configuration
{
    public interface IAspNetCoreReceiverBuilder : ITransportReceiverBuilder<IAspNetCoreReceiverBuilder>
    {
        IAspNetCoreReceiverBuilder UseRoutePrefix(PathString path);
    }
}