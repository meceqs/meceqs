using System.Threading.Tasks;

namespace Meceqs.Filters.TypedHandling
{
    public interface IHandles
    {
    }

    public interface IHandles<TMessage> : IHandles where TMessage : IMessage
    {
        Task HandleAsync(HandleContext<TMessage> context);
    }

    public interface IHandles<TMessage, TResult> : IHandles where TMessage : IMessage
    {
        Task<TResult> HandleAsync(HandleContext<TMessage> context);
    }
}