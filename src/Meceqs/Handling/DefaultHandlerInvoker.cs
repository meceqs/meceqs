using System;
using System.Threading.Tasks;

namespace Meceqs.Handling
{
    public class DefaultHandlerInvoker : IHandlerInvoker
    {
        public async Task<TResult> InvokeHandleAsync<TMessage, TResult>(IHandles<TMessage, TResult> handler, HandleContext<TMessage> context)
            where TMessage : IMessage
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return await handler.HandleAsync(context);
        }
    }
}