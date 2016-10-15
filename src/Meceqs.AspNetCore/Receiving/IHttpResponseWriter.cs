using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore.Receiving
{
    public interface IHttpResponseWriter
    {
        Task HandleResult(object result, HttpContext httpContext);
    }
}