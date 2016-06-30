using System.Threading.Tasks;

namespace Meceqs.Handling
{
    public class DefaultHandleInvoker : IHandleInvoker
    {
        public Task<TResult> InvokeHandleAsync<TMessage, TResult>(IHandles<TMessage, TResult> handler, HandleContext<TMessage> context)
            where TMessage : IMessage
        {
            Check.NotNull(handler, nameof(handler));

            return handler.HandleAsync(context);
        }
    }
}