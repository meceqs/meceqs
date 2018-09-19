using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Meceqs.Serialization;

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

            var serializer = _serializationProvider.GetDefaultSerializer();

            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

            using (MemoryStream stream = new MemoryStream())
            {
                serializer.SerializeToStream(envelope, stream);

                request.Content = new ByteArrayContent(stream.ToArray());
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(serializer.ContentType);
            }

            return request;
        }
    }
}
