using System.Net.Http;
using System.Text;
using Meceqs.Serialization;

namespace Meceqs.HttpSender
{
    public class DefaultHttpRequestMessageConverter : IHttpRequestMessageConverter
    {
        private readonly IEnvelopeSerializer _envelopeSerializer;

        public DefaultHttpRequestMessageConverter(IEnvelopeSerializer envelopeSerializer)
        {
            Check.NotNull(envelopeSerializer, nameof(envelopeSerializer));

            _envelopeSerializer = envelopeSerializer;
        }

        public HttpRequestMessage ConvertToRequestMessage(Envelope envelope, string relativePath)
        {
            Check.NotNull(envelope, nameof(envelope));
            Check.NotNullOrWhiteSpace(relativePath, nameof(relativePath));

            var request = new HttpRequestMessage(HttpMethod.Post, relativePath);

            string serializedEnvelope = _envelopeSerializer.SerializeEnvelopeToString(envelope);

            request.Content = new StringContent(serializedEnvelope, Encoding.UTF8, _envelopeSerializer.ContentType);

            return request;
        }
    }
}