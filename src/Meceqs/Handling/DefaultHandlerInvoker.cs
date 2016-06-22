using System.Threading.Tasks;

namespace Meceqs.Handling
{
    public class DefaultHandlerInvoker : IHandlerInvoker
    {
        public async Task<TResult> InvokeAsync<TMessage, TResult>(IHandles<TMessage, TResult> handler, HandleContext<TMessage> context)
            where TMessage : IMessage
        {
            return await handler.HandleAsync(context);
        }
    }
}