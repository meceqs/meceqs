using Meceqs.Filters.TypedHandling.Configuration;

namespace Meceqs.Filters.TypedHandling
{
    public class TypedHandlingOptions
    {
        public HandlerCollection Handlers { get; } = new HandlerCollection();

        public InterceptorCollection Interceptors { get; } = new InterceptorCollection();
    }
}