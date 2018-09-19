using System.Threading.Tasks;
using Meceqs.Transport;
using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore.Receiving
{
    public interface IAspNetCoreReceiver
    {
        Task ReceiveAsync(HttpContext httpContext, string receiverName, MessageMetadata metadata);
    }
}
