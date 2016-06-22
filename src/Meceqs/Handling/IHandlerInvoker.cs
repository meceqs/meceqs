using System.Threading.Tasks;

namespace Meceqs.Handling
{
    public interface IHandlerInvoker
    {
        Task<TResult> InvokeHandleAsync<TMessage, TResult>(IHandles<TMessage, TResult> handler, HandleContext<TMessage> context)
            where TMessage : IMessage;
    }
}