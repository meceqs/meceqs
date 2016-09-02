using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Meceqs.Filters.TypedHandling.Configuration
{
    public class HandleInterceptorCollection : Collection<IHandleInterceptorFactory>
    {
        public void Add<TInterceptor>() where TInterceptor : IHandleInterceptor
        {
            Add(typeof(TInterceptor));
        }

        public void Add(Type interceptorType)
        {
            Check.NotNull(interceptorType, nameof(interceptorType));

            if (!typeof(IHandleInterceptor).IsAssignableFrom(interceptorType))
            {
                throw new ArgumentException(
                    $"Type '{interceptorType}' must derive from '{typeof(IHandleInterceptor)}'",
                    nameof(interceptorType));
            }

            var factory = new ActivatorHandleInterceptorFactory(interceptorType);
            Add(factory);
        }

        public void AddService<TInterceptor>() where TInterceptor : IHandleInterceptor
        {
            AddService(typeof(TInterceptor));
        }

        public void AddService(Type interceptorType)
        {
            Check.NotNull(interceptorType, nameof(interceptorType));

            if (!typeof(IHandleInterceptor).IsAssignableFrom(interceptorType))
            {
                throw new ArgumentException(
                    $"Type '{interceptorType}' must derive from '{typeof(IHandleInterceptor)}'",
                    nameof(interceptorType));
            }

            var factory = new ServiceProviderHandleInterceptorFactory(interceptorType);
            Add(factory);
        }
    }
}