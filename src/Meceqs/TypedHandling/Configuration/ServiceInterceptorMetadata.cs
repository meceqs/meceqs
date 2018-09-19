using System;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.TypedHandling.Configuration
{
    /// <summary>
    /// Resolves an <see cref="IHandleInterceptor" /> using <see cref="IServiceProvider" />.
    /// </summary>
    public class ServiceInterceptorMetadata : IInterceptorMetadata
    {
        private readonly Type _interceptorType;

        public ServiceInterceptorMetadata(Type interceptorType)
        {
            Guard.NotNull(interceptorType, nameof(interceptorType));

            _interceptorType = interceptorType;
        }

        public IHandleInterceptor CreateInterceptor(IServiceProvider serviceProvider)
        {
            Guard.NotNull(serviceProvider, nameof(serviceProvider));

            return (IHandleInterceptor)serviceProvider.GetRequiredService(_interceptorType);
        }
    }
}
