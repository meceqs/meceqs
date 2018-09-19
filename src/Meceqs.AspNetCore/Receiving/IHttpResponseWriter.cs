using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore.Receiving
{
    public interface IHttpResponseWriter
    {
        void WriteResponse(object response, HttpContext httpContext);
    }
}
