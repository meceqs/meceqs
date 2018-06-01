using System;
using Meceqs.Serialization;
using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore.Receiving
{
    public class DefaultHttpRequestReader : IHttpRequestReader
    {
        private readonly ISerializationProvider _serializationProvider;

        public DefaultHttpRequestReader(ISerializationProvider serializationProvider)
        {
            Guard.NotNull(serializationProvider, nameof(serializationProvider));

            _serializationProvider = serializationProvider;
        }

        public Envelope ConvertToEnvelope(HttpContext httpContext, Type messageType)
        {
            Guard.NotNull(httpContext, nameof(httpContext));
            Guard.NotNull(messageType, nameof(messageType));

            // TODO error handling etc
            // TODO charset/encoding?

            var requestHeaders = httpContext.Request.GetTypedHeaders();
            string contentType = requestHeaders.ContentType.MediaType.ToString();

            return _serializationProvider.DeserializeEnvelope(contentType, httpContext.Request.Body, messageType.FullName);
        }
    }
}