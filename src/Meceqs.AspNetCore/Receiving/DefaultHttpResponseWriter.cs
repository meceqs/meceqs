using System.Collections.Generic;
using Meceqs.Serialization;
using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore.Receiving
{
    public class DefaultHttpResponseWriter : IHttpResponseWriter
    {
        private readonly ISerializationProvider _serializationProvider;

        public DefaultHttpResponseWriter(ISerializationProvider serializationProvider)
        {
            Guard.NotNull(serializationProvider, nameof(serializationProvider));

            _serializationProvider = serializationProvider;
        }

        public void WriteResponse(object response, HttpContext httpContext)
        {
            if (response == null)
                return;

            IEnumerable<string> supportedContentTypes = GetSupportedContentTypes(httpContext);

            ISerializer serializer = _serializationProvider.GetSerializer(supportedContentTypes);

            httpContext.Response.ContentType = serializer.ContentType;

            serializer.SerializeToStream(response, httpContext.Response.Body);
        }

        private IEnumerable<string> GetSupportedContentTypes(HttpContext httpContext)
        {
            var headers = httpContext.Request.GetTypedHeaders();

            if (headers.Accept != null)
            {
                foreach (var accept in headers.Accept)
                {
                    yield return accept.MediaType.ToString();
                }
            }
        }
    }
}
