using System.Threading.Tasks;
using Meceqs.Sending;
using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore
{
    public class AspNetCoreSendingMediator : IMessageSendingMediator
    {
        public const string HeaderSourceRequestId = "SourceRequestId";
        public const string HeaderSourceRequestPath = "SourceRequestPath";

        private readonly IMessageSendingMediator _inner;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AspNetCoreSendingMediator(IMessageSendingMediator inner, IHttpContextAccessor httpContextAccessor)
        {
            Check.NotNull(inner, nameof(inner));
            Check.NotNull(httpContextAccessor, nameof(httpContextAccessor));

            _inner = inner;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<TResult> SendAsync<TResult>(MessageContext context)
        {
            Check.NotNull(context, nameof(context));

            var httpContext = _httpContextAccessor.HttpContext;

            // Makes sure pending cancellations are propagated to the transport
            context.Cancellation = httpContext.RequestAborted;

            // ASP.NET Core specific tracing headers
            context.Envelope.SetHeader(HeaderSourceRequestId, httpContext.TraceIdentifier);
            context.Envelope.SetHeader(HeaderSourceRequestPath, httpContext.Request.Path);

            // TODO @cweiss CorrelationId, User?

            return _inner.SendAsync<TResult>(context);
        }
    }
}