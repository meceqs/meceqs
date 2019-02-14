using System;
using Meceqs.Internal;
using Meceqs.Serialization;
using Meceqs.Transport;
using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore.Receiving
{
    public class DefaultHttpRequestReader : IHttpRequestReader
    {
        private readonly ISerializationProvider _serializationProvider;
        private readonly IEnvelopeFactory _envelopeFactory;

        public DefaultHttpRequestReader(ISerializationProvider serializationProvider, IEnvelopeFactory envelopeFactory)
        {
            Guard.NotNull(serializationProvider, nameof(serializationProvider));
            Guard.NotNull(envelopeFactory, nameof(envelopeFactory));

            _serializationProvider = serializationProvider;
            _envelopeFactory = envelopeFactory;
        }

        public Envelope ConvertToEnvelope(HttpContext httpContext, Type messageType)
        {
            Guard.NotNull(httpContext, nameof(httpContext));
            Guard.NotNull(messageType, nameof(messageType));

            // TODO error handling etc
            // TODO charset/encoding?

            var requestHeaders = httpContext.Request.GetTypedHeaders();
            string contentType = requestHeaders.ContentType.MediaType.ToString();

            object message = _serializationProvider.Deserialize(contentType, messageType, httpContext.Request.Body);

            Guid messageId = GetGuidHeader(requestHeaders.Headers, TransportHeaderNames.MessageId) ?? Guid.NewGuid();

            Envelope envelope = _envelopeFactory.Create(message, messageId);

            envelope.CorrelationId = GetGuidHeader(requestHeaders.Headers, TransportHeaderNames.CorrelationId) ?? envelope.MessageId;

            foreach (string key in requestHeaders.Headers.Keys)
            {
                if (key.StartsWith(TransportHeaderNames.HeaderPrefix))
                {
                    string headerKey = key.Substring(TransportHeaderNames.HeaderPrefix.Length);
                    envelope.Headers[headerKey] = requestHeaders.Headers[headerKey].ToString();
                }
            }

            return envelope;
        }

        private Guid? GetGuidHeader(IHeaderDictionary headers, string key)
        {
            if (headers.TryGetValue(key, out var value))
            {
                if (Guid.TryParse(value, out var guid))
                {
                    if (guid != Guid.Empty)
                    {
                        return guid;
                    }
                }
            }

            return null;
        }
    }
}
