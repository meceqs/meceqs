using System;
using System.Threading;
using Meceqs.Pipeline;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Meceqs.AspNetCore
{
    public class AspNetCoreEnricher : IFilterContextEnricher
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

        public void EnrichFilterContext(FilterContext context)
        {
            Check.NotNull(context, nameof(context));

            var httpContext = _httpContextAccessor.HttpContext;

            if (context.RequestServices == null)
            {
                context.RequestServices = httpContext.RequestServices;
            }

            if (context.User == null)
            {
                context.User = httpContext.User;
            }

            if (context.Cancellation == default(CancellationToken))
            {
                context.Cancellation = httpContext.RequestAborted;
            }
            else if (context.Cancellation != httpContext.RequestAborted)
            {
                // Someone provided a custom cancellation. To make sure the operation still is cancelled
                // when the ASP.NET request is cancelled, the two cancellation tokens are combined.
                var compositeCancellation = CancellationTokenSource.CreateLinkedTokenSource(
                    context.Cancellation,
                    httpContext.RequestAborted
                );
                context.Cancellation = compositeCancellation.Token;
            }

            AddRemoteUserHeaders(context, httpContext);
            AddHistoryEntry(context, httpContext);
        }

        private void AddRemoteUserHeaders(FilterContext filterContext, HttpContext httpContext)
        {
            if (!_options.AddRemoteUserHeaders)
                return;

            if (!filterContext.Envelope.Headers.ContainsKey(_options.RemoteUserIpAddressHeaderName))
            {
                filterContext.Envelope.Headers.Add(_options.RemoteUserIpAddressHeaderName, httpContext.Connection?.RemoteIpAddress.ToString());
            }

            if (!filterContext.Envelope.Headers.ContainsKey(_options.RemoteUserAgentHeaderName))
            {
                filterContext.Envelope.Headers.Add(_options.RemoteUserAgentHeaderName, httpContext.Request.Headers["User-Agent"].ToString());
            }
        }

        private void AddHistoryEntry(FilterContext filterContext, HttpContext httpContext)
        {
            var historyEntry = new EnvelopeHistoryEntry
            {
                Pipeline = filterContext.PipelineName,
                Host = _options.HostName,
                Endpoint = _options.EndpointName,
                CreatedOnUtc = DateTime.UtcNow
            };

            historyEntry.Properties.Add(_options.HistoryPropertyRequestId, httpContext.TraceIdentifier);
            historyEntry.Properties.Add(_options.HistoryPropertyRequestPath, httpContext.Request.Path.Value);

            filterContext.Envelope.History.Add(historyEntry);
        }
    }
}