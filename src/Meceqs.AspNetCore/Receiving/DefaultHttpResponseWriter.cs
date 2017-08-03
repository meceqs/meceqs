using System.Threading.Tasks;
using Meceqs.Serialization;
using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore.Receiving
{
    public class DefaultHttpResponseWriter : IHttpResponseWriter
    {
        private readonly IResultSerializer _resultSerializer;

        public DefaultHttpResponseWriter(IResultSerializer resultSerializer)
        {
            Guard.NotNull(resultSerializer, nameof(resultSerializer));

            _resultSerializer = resultSerializer;
        }

        public Task HandleResult(object result, HttpContext httpContext)
        {
            if (result == null)
                return Task.CompletedTask;

            httpContext.Response.ContentType = _resultSerializer.ContentType;

            string response = _resultSerializer.SerializeResultToString(result);

            return httpContext.Response.WriteAsync(response);
        }
    }
}