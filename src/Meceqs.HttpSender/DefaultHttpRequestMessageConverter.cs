using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Meceqs.Serialization;
using Meceqs.Transport;

namespace Meceqs.HttpSender
{
    public class DefaultHttpRequestMessageConverter : IHttpRequestMessageConverter
    {
        private readonly ISerializationProvider _serializationProvider;

        public DefaultHttpRequestMessageConverter(ISerializationProvider serializationProvider)
        {
            Guard.NotNull(serializationProvider, nameof(serializationProvider));

            _serializationProvider = serializationProvider;
        }

        public HttpRequestMessage ConvertToRequestMessage(Envelope envelope, Uri requestUri)
        {
            Guard.NotNull(envelope, nameof(envelope));
            Guard.NotNull(requestUri, nameof(requestUri));

            var serializer = _serializationProvider.GetSerializer(envelope.Message.GetType());

            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

            using (MemoryStream stream = new MemoryStream())
            {
                serializer.SerializeToStream(envelope.Message, stream);

                request.Content = new ByteArrayContent(stream.ToArray());
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(serializer.ContentType);
                
                request.Content.Headers.Add(TransportHeaderNames.MessageId, envelope.MessageId.ToString());

                if (envelope.CorrelationId != envelope.MessageId)
                {
                    request.Content.Headers.Add(TransportHeaderNames.CorrelationId, envelope.CorrelationId.ToString());
                }

                foreach (var headerEntry in envelope.Headers)
                {
                    request.Content.Headers.Add(TransportHeaderNames.HeaderPrefix + headerEntry.Key, headerEntry.Value.ToString());
                }

                // We also need to let the receiver know what result types we can handle.
                foreach (var acceptedContentType in _serializationProvider.SupportedContentTypes)
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptedContentType));
                }
            }

            return request;
        }
    }
}
