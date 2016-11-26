using System;
using System.Threading;
using Meceqs.Pipeline;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Meceqs.AspNetCore
{
    public class AspNetCoreEnricher : IMessageContextEnricher
    {
        private readonly AspNetCoreEnricherOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AspNetCoreEnricher(
            IOptions<AspNetCoreEnricherOptions> options,
            IHttpContextAccessor httpContextAccessor)
        {
            Check.NotNull(options?.Value, nameof(options));
            Check.NotNull(httpContextAccessor, nameof(httpContextAccessor));

            _options = options.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public void EnrichMessageContext(MessageContext context)
        {
            Check.NotNull(context, nameof(context));

            var httpContext = _httpContextAccessor.HttpContext;

            AttachToHttpContext(context, httpContext);

            AddRemoteUserHeaders(context, httpContext);

            AddHistoryEntry(context, httpContext);
        }

        private void AttachToHttpContext(MessageContext messageContext, HttpContext httpContext)
        {
            // HttpContext is null e.g. when a message is sent on a background thread.
            if (httpContext == null)
                return;

            if (messageContext.User == null)
            {
                messageContext.User = httpContext.User;
            }

            if (messageContext.Cancellation == default(CancellationToken))
            {
                messageContext.Cancellation = httpContext.RequestAborted;
            }
            else if (messageContext.Cancellation != httpContext.RequestAborted)
            {
                // Someone provided a custom cancellation. To make sure the operation still is cancelled
                // when the ASP.NET request is cancelled, the two cancellation tokens are combined.
                var compositeCancellation = CancellationTokenSource.CreateLinkedTokenSource(
                    messageContext.Cancellation,
                    httpContext.RequestAborted
                );
                messageContext.Cancellation = compositeCancellation.Token;
            }
        }

        private void AddRemoteUserHeaders(MessageContext messageContext, HttpContext httpContext)
        {
            if (!_options.AddRemoteUserHeaders || httpContext == null)
                return;

            if (!messageContext.Envelope.Headers.ContainsKey(_options.RemoteUserIpAddressHeaderName))
            {
                messageContext.Envelope.Headers.Add(_options.RemoteUserIpAddressHeaderName, httpContext.Connection?.RemoteIpAddress.ToString());
            }

            if (!messageContext.Envelope.Headers.ContainsKey(_options.RemoteUserAgentHeaderName))
            {
                StringValues userAgent;
                if (httpContext.Request.Headers.TryGetValue("User-Agent", out userAgent))
                {
                    messageContext.Envelope.Headers.Add(_options.RemoteUserAgentHeaderName, userAgent.ToString());
                }
            }
        }

        private void AddHistoryEntry(MessageContext messageContext, HttpContext httpContext)
        {
            var historyEntry = new EnvelopeHistoryEntry
            {
                Pipeline = messageContext.PipelineName,
                Host = _options.HostName,
                Endpoint = _options.EndpointName,
                CreatedOnUtc = DateTime.UtcNow
            };

            if (httpContext != null)
            {
                historyEntry.Properties.Add(_options.HistoryPropertyRequestId, httpContext.TraceIdentifier);
                historyEntry.Properties.Add(_options.HistoryPropertyRequestPath, httpContext.Request.Path.Value);
            }

            messageContext.Envelope.History.Add(historyEntry);
        }
    }
}