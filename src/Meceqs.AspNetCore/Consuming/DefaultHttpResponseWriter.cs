using System.Threading.Tasks;
using Meceqs.Serialization;
using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore.Consuming
{
    public class DefaultHttpResponseWriter : IHttpResponseWriter
    {
        private readonly IResultSerializer _resultSerializer;

        public DefaultHttpResponseWriter(IResultSerializer resultSerializer)
        {
            Check.NotNull(resultSerializer, nameof(resultSerializer));

            _resultSerializer = resultSerializer;
        }

        public Task HandleResult(object result, HttpContext httpContext)
        {
            if (result == null)
                return Task.CompletedTask;

            httpContext.Response.ContentType = _resultSerializer.ContentType;

            string response = _resultSerializer.SerializeToString(result);

            return httpContext.Response.WriteAsync(response);
        }
    }
}