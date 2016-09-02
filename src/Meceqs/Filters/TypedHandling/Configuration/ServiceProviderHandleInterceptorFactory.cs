using System;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Filters.TypedHandling.Configuration
{
    public class ServiceProviderHandleInterceptorFactory : IHandleInterceptorFactory
    {
        private readonly Type _interceptorType;

        public ServiceProviderHandleInterceptorFactory(Type interceptorType)
        {
            Check.NotNull(interceptorType, nameof(interceptorType));

            _interceptorType = interceptorType;
        }

        public IHandleInterceptor CreateInterceptor(IServiceProvider serviceProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            return (IHandleInterceptor)serviceProvider.GetRequiredService(_interceptorType);
        }
    }
}