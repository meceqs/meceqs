using System.Threading.Tasks;

namespace Meceqs.Handling
{
    public interface IHandles<TMessage, TResult> where TMessage : IMessage
    {
        Task<TResult> HandleAsync(HandleContext<TMessage> context);
    }
}