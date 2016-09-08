using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore.Consuming
{
    public interface IAspNetCoreConsumer
    {
        Task HandleAsync(HttpContext httpContext, MessageMetadata metadata);
    }
}