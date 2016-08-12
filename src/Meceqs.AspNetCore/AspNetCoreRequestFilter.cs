using System;
using System.Reflection;
using System.Threading.Tasks;
using Meceqs.Pipeline;
using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore
{
    public class AspNetCoreRequestFilter
    {
        public const string HistoryPropertyRequestId = "RequestId";
        public const string HistoryPropertyRequestPath = "RequestPath";

        private static readonly string HostName = Environment.MachineName;
        private static readonly string EndpointName = Assembly.GetEntryAssembly().GetName().Name;

        private readonly FilterDelegate _next;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AspNetCoreRequestFilter(FilterDelegate next, IHttpContextAccessor httpContextAccessor)
        {
            Check.NotNull(next, nameof(next));
            Check.NotNull(httpContextAccessor, nameof(httpContextAccessor));

            _next = next;
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
                Host = HostName,
                Endpoint = EndpointName,
                CreatedOnUtc = DateTime.UtcNow
            };

            historyEntry.Properties.Add(HistoryPropertyRequestId, httpContext.TraceIdentifier);
            historyEntry.Properties.Add(HistoryPropertyRequestPath, httpContext.Request.Path.Value);

            context.Envelope.MessageHistory.Add(historyEntry);

            return _next(context);
        }
    }
}