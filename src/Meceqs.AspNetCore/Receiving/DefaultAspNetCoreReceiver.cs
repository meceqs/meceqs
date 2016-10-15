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
            Check.NotNull(httpRequestReader, nameof(httpRequestReader));
            Check.NotNull(messageReceiver, nameof(messageReceiver));
            Check.NotNull(httpResponseWriter, nameof(httpResponseWriter));

            _httpRequestReader = httpRequestReader;
            _messageReceiver = messageReceiver;
            _httpResponseWriter = httpResponseWriter;
        }

        public async Task ReceiveAsync(HttpContext httpContext, MessageMetadata metadata)
        {
            // TODO error handling etc.

            var envelope = _httpRequestReader.ConvertToEnvelope(httpContext, metadata.MessageType);

            object result = await _messageReceiver.ForEnvelope(envelope)
                .SetCancellationToken(httpContext.RequestAborted)
                .ReceiveAsync(metadata.ResultType);

            await _httpResponseWriter.HandleResult(result, httpContext);
        }
    }
}