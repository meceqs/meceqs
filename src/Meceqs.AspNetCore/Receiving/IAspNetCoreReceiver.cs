using System.Threading.Tasks;
using Meceqs.Transport;
using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore.Consuming
{
    public interface IAspNetCoreConsumer
    {
        Task ConsumeAsync(HttpContext httpContext, MessageMetadata metadata);
    }
}