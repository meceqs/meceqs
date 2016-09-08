using Meceqs.Configuration;
using Meceqs.TypedHandling.Configuration;

namespace Meceqs.TypedHandling
{
    public class TypedHandlingOptions
    {
        public HandlerCollection Handlers { get; } = new HandlerCollection();

        public InterceptorCollection Interceptors { get; } = new InterceptorCollection();

        public UnknownMessageBehavior UnknownMessageBehavior { get; set; } = UnknownMessageBehavior.ThrowException;
    }
}