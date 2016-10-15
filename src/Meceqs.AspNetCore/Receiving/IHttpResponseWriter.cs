using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore.Consuming
{
    public interface IHttpResponseWriter
    {
        Task HandleResult(object result, HttpContext httpContext);
    }
}