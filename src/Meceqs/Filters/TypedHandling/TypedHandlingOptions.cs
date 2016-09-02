using Meceqs.Filters.TypedHandling.Configuration;

namespace Meceqs.Filters.TypedHandling
{
    public class TypedHandlingOptions
    {
        public HandleInterceptorCollection Interceptors { get; } = new HandleInterceptorCollection();

    }
}