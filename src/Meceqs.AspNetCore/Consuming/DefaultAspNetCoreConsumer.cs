using System.Threading.Tasks;
using Meceqs.Consuming;
using Meceqs.Transport;
using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore.Consuming
{
    public class DefaultAspNetCoreConsumer : IAspNetCoreConsumer
    {
        private readonly IHttpRequestReader _httpRequestReader;
        private readonly IMessageConsumer _messageConsumer;
        private readonly IHttpResponseWriter _httpResponseWriter;

        public DefaultAspNetCoreConsumer(
            IHttpRequestReader httpRequestReader,
            IMessageConsumer messageConsumer,
            IHttpResponseWriter httpResponseWriter)
        { 
            Check.NotNull(httpRequestReader, nameof(httpRequestReader));
            Check.NotNull(messageConsumer, nameof(messageConsumer));
            Check.NotNull(httpResponseWriter, nameof(httpResponseWriter));

            _httpRequestReader = httpRequestReader;
            _messageConsumer = messageConsumer;
            _httpResponseWriter = httpResponseWriter;
        }

        public async Task HandleAsync(HttpContext httpContext, MessageMetadata metadata)
        {
            // TODO error handling etc.

            var envelope = _httpRequestReader.ConvertToEnvelope(httpContext, metadata.MessageType);

            object result = await _messageConsumer.ForEnvelope(envelope)
                .SetCancellationToken(httpContext.RequestAborted)
                .SetRequestServices(httpContext.RequestServices)
                .ConsumeAsync(metadata.ResultType);

            await _httpResponseWriter.HandleResult(result, httpContext);
        }
    }
}