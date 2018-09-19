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
    /// Represents a handler that processes the given message but does not return a response.
    /// </summary>
    public interface IHandles<TMessage> : IHandles where TMessage : class
    {
        Task HandleAsync(TMessage message, HandleContext context);
    }

    /// <summary>
    /// Represents a handler that processes the given message and returns a response of the given type.
    /// </summary>
    public interface IHandles<TMessage, TResponse> : IHandles where TMessage : class
    {
        Task<TResponse> HandleAsync(TMessage message, HandleContext context);
    }
}
