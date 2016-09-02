using System;

namespace Meceqs.Filters.TypedHandling.Configuration
{
    public interface IHandleInterceptorFactory
    {
        IHandleInterceptor CreateInterceptor(IServiceProvider serviceProvider);
    }
}