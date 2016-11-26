using System.Threading.Tasks;

namespace Meceqs.TypedHandling
{
    /// <summary>
    /// <para>Interceptors are wrapped around the handler. This allows interceptors to execute logic
    /// before or after the handler is executed.</para>
    /// <para>The main difference between interceptors and "middleware" is, that interceptors have access to the
    /// actual handler class and "Handle"-method - this allows them to e.g. access custom attributes.</para>
    /// </summary>
    public interface IHandleInterceptor
    {
        /// <summary>
        /// An interceptor can execute logic before or after the handler is executed.
        /// You have to invoke <paramref name="next"/> or the handler will not be executed.
        /// </summary>
        Task OnHandleExecutionAsync(HandleContext context, HandleExecutionDelegate next);
    }
}