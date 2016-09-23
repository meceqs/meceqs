using System.Threading.Tasks;

namespace Meceqs.TypedHandling
{
    /// <summary>
    /// Represents a pointer to the next call in a chain of interceptors and the actual handler.
    /// </summary>
    public delegate Task HandleExecutionDelegate(HandleContext context);
}