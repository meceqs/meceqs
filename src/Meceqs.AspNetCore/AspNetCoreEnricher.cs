using System.Threading;
using Meceqs.Pipeline;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

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
            Guard.NotNull(options?.Value, nameof(options));
            Guard.NotNull(httpContextAccessor, nameof(httpContextAccessor));

            _options = options.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public void EnrichMessageContext(MessageContext context)
        {
            Guard.NotNull(context, nameof(context));

            var httpContext = _httpContextAccessor.HttpContext;

            AttachToHttpContext(context, httpContext);

            AddRemoteUserHeaders(context, httpContext);
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
                if (httpContext.Request.Headers.TryGetValue("User-Agent", out var userAgent))
                {
                    messageContext.Envelope.Headers.Add(_options.RemoteUserAgentHeaderName, userAgent.ToString());
                }
            }
        }
    }
}
