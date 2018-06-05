using Meceqs.Transport;
using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore.Receiving
{
    public class AspNetCoreReceiverOptions : TransportReceiverOptions
    {
        public PathString RoutePrefix { get; set; }

        public AspNetCoreReceiverOptions()
        {
            // Default options

            // We give other ASP.NET Core middleware a chance to process it or to return a 404.
            UnknownMessageBehavior = UnknownMessageBehavior.Skip;
        }
    }
}