using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore.Receiving
{
    public interface IHttpResponseWriter
    {
        void WriteResult(object result, HttpContext httpContext);
    }
}