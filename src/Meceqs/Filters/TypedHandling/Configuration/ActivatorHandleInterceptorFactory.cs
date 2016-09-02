using System;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Filters.TypedHandling.Configuration
{
    public class ActivatorHandleInterceptorFactory : IHandleInterceptorFactory
    {
        private readonly Type _interceptorType;

        public ActivatorHandleInterceptorFactory(Type interceptorType)
        {
            Check.NotNull(interceptorType, nameof(interceptorType));

            _interceptorType = interceptorType;
        }

        public IHandleInterceptor CreateInterceptor(IServiceProvider serviceProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            return (IHandleInterceptor)ActivatorUtilities.CreateInstance(serviceProvider, _interceptorType);
        }
    }
}