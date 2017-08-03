using System;
using Meceqs.Serialization;
using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore.Receiving
{
    public class DefaultHttpRequestReader : IHttpRequestReader
    {
        private readonly IEnvelopeDeserializer _envelopeDeserializer;

        public DefaultHttpRequestReader(IEnvelopeDeserializer envelopeDeserializer)
        {
            Guard.NotNull(envelopeDeserializer, nameof(envelopeDeserializer));

            _envelopeDeserializer = envelopeDeserializer;
        }

        public Envelope ConvertToEnvelope(HttpContext httpContext, Type messageType)
        {
            Guard.NotNull(httpContext, nameof(httpContext));
            Guard.NotNull(messageType, nameof(messageType));

            // TODO error handling etc
            // TODO charset/encoding?

            var requestHeaders = httpContext.Request.GetTypedHeaders();

            // TODO @cweiss should this get the actual messageType object?
            Envelope envelope = _envelopeDeserializer.DeserializeEnvelope(
                requestHeaders.ContentType.MediaType,
                httpContext.Request.Body,
                messageType.FullName);

            return envelope;
        }
    }
}