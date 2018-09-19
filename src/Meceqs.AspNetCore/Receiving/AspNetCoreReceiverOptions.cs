using Meceqs.Transport;
using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore.Receiving
{
    public class AspNetCoreReceiverOptions : ReceiveTransportOptions
    {
        public PathString RoutePrefix { get; set; }

        public bool RequiresAuthentication { get; set; } = true;

        public AspNetCoreReceiverOptions()
        {
            // We give other ASP.NET Core middleware components a chance to process it or to return a 404.
            UnknownMessageBehavior = UnknownMessageBehavior.Skip;
        }
    }
}
