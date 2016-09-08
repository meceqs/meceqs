using Meceqs.Pipeline;

namespace Meceqs.TypedHandling.Internal
{
    public interface IHandleContextFactory
    {
        HandleContext CreateHandleContext(FilterContext filterContext);
    }
}