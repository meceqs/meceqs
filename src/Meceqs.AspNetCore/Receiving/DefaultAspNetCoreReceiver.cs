using System.Threading.Tasks;
using Meceqs.Receiving;
using Meceqs.Transport;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Meceqs.AspNetCore.Receiving
{
    public class DefaultAspNetCoreReceiver : IAspNetCoreReceiver
    {
        private readonly IOptionsMonitor<AspNetCoreReceiverOptions> _optionsMonitor;
        private readonly IHttpRequestReader _httpRequestReader;
        private readonly IMessageReceiver _messageReceiver;
        private readonly IHttpResponseWriter _httpResponseWriter;

        public DefaultAspNetCoreReceiver(
            IOptionsMonitor<AspNetCoreReceiverOptions> optionsMonitor,
            IHttpRequestReader httpRequestReader,
            IMessageReceiver messageReceiver,
            IHttpResponseWriter httpResponseWriter)
        {
            Guard.NotNull(optionsMonitor, nameof(optionsMonitor));
            Guard.NotNull(httpRequestReader, nameof(httpRequestReader));
            Guard.NotNull(messageReceiver, nameof(messageReceiver));
            Guard.NotNull(httpResponseWriter, nameof(httpResponseWriter));

            _optionsMonitor = optionsMonitor;
            _httpRequestReader = httpRequestReader;
            _messageReceiver = messageReceiver;
            _httpResponseWriter = httpResponseWriter;
        }

        public async Task ReceiveAsync(HttpContext httpContext, string receiverName, MessageMetadata metadata)
        {
            // TODO error handling etc.

            var options = _optionsMonitor.Get(receiverName);

            if (options.RequiresAuthentication && httpContext.User?.Identity?.IsAuthenticated != true)
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            else
            {
                var envelope = _httpRequestReader.ConvertToEnvelope(httpContext, metadata.MessageType);

                object response = await _messageReceiver.ForEnvelope(envelope)
                    .UsePipeline(receiverName)
                    .SetCancellationToken(httpContext.RequestAborted)
                    .SetUser(httpContext.User)
                    .ReceiveAsync(metadata.ResponseType);

                _httpResponseWriter.WriteResponse(response, httpContext);
            }
        }
    }
}