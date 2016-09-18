using System.Threading.Tasks;

namespace Meceqs.TypedHandling
{
    public interface IHandles
    {
    }

    public interface IHandles<TMessage> : IHandles where TMessage : class
    {
        Task HandleAsync(HandleContext<TMessage> context);
    }

    public interface IHandles<TMessage, TResult> : IHandles where TMessage : class
    {
        Task<TResult> HandleAsync(HandleContext<TMessage> context);
    }
}