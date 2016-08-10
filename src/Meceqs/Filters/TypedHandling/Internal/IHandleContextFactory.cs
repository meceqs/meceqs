using Meceqs.Pipeline;

namespace Meceqs.Filters.TypedHandling.Internal
{
    public interface IHandleContextFactory
    {
        HandleContext CreateHandleContext(FilterContext filterContext);
    }
}