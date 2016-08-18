using System;
using System.Threading.Tasks;
using Meceqs.Pipeline;
using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore
{
    public class AspNetCoreRequestFilter
    {
        private readonly FilterDelegate _next;
        private readonly AspNetCoreRequestOptions _options;

        public AspNetCoreRequestFilter(FilterDelegate next, AspNetCoreRequestOptions options)
        {
            Check.NotNull(next, nameof(next));
            Check.NotNull(options, nameof(options));

            _next = next;
            _options = options;
        }

        public Task Invoke(FilterContext context, IHttpContextAccessor httpContextAccessor)
        {
            Check.NotNull(context, nameof(context));
            Check.NotNull(httpContextAccessor, nameof(httpContextAccessor));

            var httpContext = httpContextAccessor.HttpContext;

            context.Cancellation = httpContext.RequestAborted;
            context.RequestServices = httpContext.RequestServices;
            context.User = httpContext.User;

            AddRemoteUserHeaders(context, httpContext);
            AddHistoryEntry(context, httpContext);

            return _next(context);
        }

        private void AddRemoteUserHeaders(FilterContext filterContext, HttpContext httpContext)
        {
            if (!_options.AddRemoteUserHeaders)
                return;

            if (!filterContext.Envelope.Headers.ContainsKey(_options.RemoteUserIpAddressHeaderName))
            {
                filterContext.Envelope.Headers.Set(_options.RemoteUserIpAddressHeaderName, httpContext.Connection?.RemoteIpAddress.ToString());
            }

            if (!filterContext.Envelope.Headers.ContainsKey(_options.RemoteUserAgentHeaderName))
            {
                filterContext.Envelope.Headers.Set(_options.RemoteUserAgentHeaderName, httpContext.Request.Headers["User-Agent"].ToString());
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