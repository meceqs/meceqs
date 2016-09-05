using Meceqs.Filters.TypedHandling.Configuration;

namespace Meceqs.Filters.TypedHandling
{
    public class TypedHandlingOptions
    {
        public InterceptorCollection Interceptors { get; } = new InterceptorCollection();
    }
}