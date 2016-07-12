using System.Threading.Tasks;

namespace Meceqs.Handling
{
    public class DefaultHandlerInvoker : IHandlerInvoker
    {
        public Task<TResult> InvokeHandleAsync<TMessage, TResult>(IHandler<TMessage, TResult> handler, MessageContext<TMessage> context)
            where TMessage : IMessage
        {
            Check.NotNull(handler, nameof(handler));

            return handler.HandleAsync(context);
        }
    }
}