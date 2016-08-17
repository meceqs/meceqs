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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AspNetCoreRequestFilter(FilterDelegate next, AspNetCoreRequestOptions options, IHttpContextAccessor httpContextAccessor)
        {
            Check.NotNull(next, nameof(next));
            Check.NotNull(options, nameof(options));
            Check.NotNull(httpContextAccessor, nameof(httpContextAccessor));

            _next = next;
            _options = options;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task Invoke(FilterContext context)
        {
            Check.NotNull(context, nameof(context));

            var httpContext = _httpContextAccessor.HttpContext;

            context.Cancellation = httpContext.RequestAborted;
            context.RequestServices = httpContext.RequestServices;
            context.User = httpContext.User;

            // attach diagnostic information to each event

            var historyEntry = new MessageHistoryEntry
            {
                Pipeline = context.PipelineName,
                Host = _options.HostName,
                Endpoint = _options.EndpointName,
                CreatedOnUtc = DateTime.UtcNow
            };

            historyEntry.Properties.Add(_options.HistoryPropertyRequestId, httpContext.TraceIdentifier);
            historyEntry.Properties.Add(_options.HistoryPropertyRequestPath, httpContext.Request.Path.Value);

            context.Envelope.History.Add(historyEntry);

            return _next(context);
        }
    }
}