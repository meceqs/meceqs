using System;
using Meceqs.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Meceqs.AspNetCore.Consuming
{
    public class DefaultHttpRequestReader : IHttpRequestReader
    {
        private readonly IEnvelopeDeserializer _envelopeDeserializer;

        public DefaultHttpRequestReader(IEnvelopeDeserializer envelopeDeserializer)
        {
            Check.NotNull(envelopeDeserializer, nameof(envelopeDeserializer));

            _envelopeDeserializer = envelopeDeserializer;
        }

        public Envelope ConvertToEnvelope(HttpContext httpContext, Type messageType)
        {
            Check.NotNull(httpContext, nameof(httpContext));
            Check.NotNull(messageType, nameof(messageType));

            // TODO error handling etc
            // TODO charset/encoding?

            var mediaType = MediaTypeHeaderValue.Parse(httpContext.Request.ContentType);
            
            // TODO @cweiss should this get the actual messageType object?
            Envelope envelope = _envelopeDeserializer.DeserializeFromStream(
                mediaType.MediaType,
                httpContext.Request.Body,
                messageType.FullName);

            return envelope;
        }
    }
}