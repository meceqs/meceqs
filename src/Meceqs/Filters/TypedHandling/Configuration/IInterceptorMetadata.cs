using System;

namespace Meceqs.Filters.TypedHandling.Configuration
{
    public interface IInterceptorMetadata
    {
        IHandleInterceptor CreateInterceptor(IServiceProvider serviceProvider);
    }
}