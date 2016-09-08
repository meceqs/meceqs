using System;

namespace Meceqs.TypedHandling.Configuration
{
    /// <summary>
    /// Describes an <see cref="IHandleInterceptor"/> and knows how to create it.
    /// </summary>
    public interface IInterceptorMetadata
    {
        IHandleInterceptor CreateInterceptor(IServiceProvider serviceProvider);
    }
}