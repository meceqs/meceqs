using System.Threading.Tasks;

namespace Meceqs.Filters.TypedHandling
{
    public interface IHandles<TMessage, TResult> where TMessage : IMessage
    {
        Task<TResult> HandleAsync(HandleContext<TMessage> context);
    }
}