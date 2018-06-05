using System.Threading.Tasks;
using Meceqs.Receiving;
using Meceqs.Transport;
using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore.Receiving
{
    public class DefaultAspNetCoreReceiver : IAspNetCoreReceiver
    {
        private readonly IHttpRequestReader _httpRequestReader;
        private readonly IMessageReceiver _messageReceiver;
        private readonly IHttpResponseWriter _httpResponseWriter;

        public DefaultAspNetCoreReceiver(
            IHttpRequestReader httpRequestReader,
            IMessageReceiver messageReceiver,
            IHttpResponseWriter httpResponseWriter)
        {
            Guard.NotNull(httpRequestReader, nameof(httpRequestReader));
            Guard.NotNull(messageReceiver, nameof(messageReceiver));
            Guard.NotNull(httpResponseWriter, nameof(httpResponseWriter));

            _httpRequestReader = httpRequestReader;
            _messageReceiver = messageReceiver;
            _httpResponseWriter = httpResponseWriter;
        }

        public async Task ReceiveAsync(HttpContext httpContext, string receiverName, MessageMetadata metadata)
        {
            // TODO error handling etc.

            var envelope = _httpRequestReader.ConvertToEnvelope(httpContext, metadata.MessageType);

            object result = await _messageReceiver.ForEnvelope(envelope)
                .UsePipeline(receiverName)
                .SetCancellationToken(httpContext.RequestAborted)
                .ReceiveAsync(metadata.ResultType);

            _httpResponseWriter.WriteResult(result, httpContext);
        }
    }
}