using System;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.TypedHandling.Configuration
{
    /// <summary>
    /// Resolves an <see cref="IHandleInterceptor" /> using <see cref="ActivatorUtilities" />.
    /// </summary>
    public class ActivatorInterceptorMetadata : IInterceptorMetadata
    {
        private readonly Type _interceptorType;

        public ActivatorInterceptorMetadata(Type interceptorType)
        {
            Guard.NotNull(interceptorType, nameof(interceptorType));

            _interceptorType = interceptorType;
        }

        public IHandleInterceptor CreateInterceptor(IServiceProvider serviceProvider)
        {
            Guard.NotNull(serviceProvider, nameof(serviceProvider));

            return (IHandleInterceptor)ActivatorUtilities.CreateInstance(serviceProvider, _interceptorType);
        }
    }
}
