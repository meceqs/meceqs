using System;
using System.Threading.Tasks;
using Meceqs.Sending;
using Meceqs.Sending.Transport;
using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore
{
    public class AspNetCoreSendTransportMediator : ISendTransportMediator
    {
        private readonly ISendTransportMediator _inner;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AspNetCoreSendTransportMediator(ISendTransportMediator inner, IHttpContextAccessor httpContextAccessor)
        {
            if (inner == null)
                throw new ArgumentNullException(nameof(inner));

            if (httpContextAccessor == null)
                throw new ArgumentNullException(nameof(httpContextAccessor));

            _inner = inner;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TResult> SendAsync<TMessage, TResult>(SendContext<TMessage> context) where TMessage : IMessage
        {
            // Makes sure pending cancellations are propagated to the transport
            context.Cancellation = _httpContextAccessor.HttpContext.RequestAborted;

            // TODO @cweiss CorrelationId, Trace Headers (Request-Url, etc), User?

            return await _inner.SendAsync<TMessage, TResult>(context);
        }
    }
}