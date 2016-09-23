using System.Threading.Tasks;

namespace Meceqs.TypedHandling
{
    /// <summary>
    /// Marker interface for a handler.
    /// </summary>
    public interface IHandles
    {
    }

    /// <summary>
    /// Represents a handler that process the given message but does not return a result.
    /// </summary>
    public interface IHandles<TMessage> : IHandles where TMessage : class
    {
        Task HandleAsync(HandleContext<TMessage> context);
    }

    /// <summary>
    /// Represents a handler that process the given message and returns a result of the given type.
    /// </summary>
    public interface IHandles<TMessage, TResult> : IHandles where TMessage : class
    {
        Task<TResult> HandleAsync(HandleContext<TMessage> context);
    }
}