using System.Threading.Tasks;

namespace Meceqs.Handling
{
    public interface IHandlerInvoker
    {
        Task<TResult> InvokeHandleAsync<TMessage, TResult>(IHandler<TMessage, TResult> handler, MessageContext<TMessage> context)
            where TMessage : IMessage;
    }
}