using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Meceqs.Filters.TypedHandling.Configuration
{
    public class InterceptorCollection : Collection<IInterceptorMetadata>
    {
        /// <summary>
        /// Adds a type representing an <see cref="IHandleInterceptor"/>.
        /// </summary>
        /// <remarks>
        /// Interceptor instances will be created using
        /// <see cref="Microsoft.Extensions.DependencyInjection.ActivatorUtilities"/>.
        /// Use <see cref="AddService(Type)"/> to register a service as an interceptor.
        /// </remarks>
        public void Add<TInterceptor>() where TInterceptor : IHandleInterceptor
        {
            Add(typeof(TInterceptor));
        }

        /// <summary>
        /// Adds a type representing an <see cref="IHandleInterceptor"/>.
        /// </summary>
        /// <param name="interceptorType">Type representing an <see cref="IHandleInterceptor"/>.</param>
        /// <remarks>
        /// Interceptor instances will be created using
        /// <see cref="Microsoft.Extensions.DependencyInjection.ActivatorUtilities"/>.
        /// Use <see cref="AddService(Type)"/> to register a service as an interceptor.
        /// </remarks>
        public void Add(Type interceptorType)
        {
            Check.NotNull(interceptorType, nameof(interceptorType));

            if (!typeof(IHandleInterceptor).IsAssignableFrom(interceptorType))
            {
                throw new ArgumentException(
                    $"Type '{interceptorType}' must derive from '{typeof(IHandleInterceptor)}'",
                    nameof(interceptorType));
            }

            var factory = new ActivatorInterceptorMetadata(interceptorType);
            Add(factory);
        }

        /// <summary>
        /// Adds a type representing an <see cref="IHandleInterceptor"/>.
        /// </summary>
        /// <remarks>
        /// Interceptor instances will be created through dependency injection. Use
        /// <see cref="Add(Type)"/> to register an interceptor that will be created via
        /// type activation.
        /// </remarks>
        public void AddService<TInterceptor>() where TInterceptor : IHandleInterceptor
        {
            AddService(typeof(TInterceptor));
        }

        /// <summary>
        /// Adds a type representing an <see cref="IHandleInterceptor"/>.
        /// </summary>
        /// <param name="interceptorType">Type representing an <see cref="IHandleInterceptor"/>.</param>
        /// <remarks>
        /// Interceptor instances will be created through dependency injection. Use
        /// <see cref="Add(Type)"/> to register an interceptor that will be created via
        /// type activation.
        /// </remarks>
        public void AddService(Type interceptorType)
        {
            Check.NotNull(interceptorType, nameof(interceptorType));

            if (!typeof(IHandleInterceptor).IsAssignableFrom(interceptorType))
            {
                throw new ArgumentException(
                    $"Type '{interceptorType}' must derive from '{typeof(IHandleInterceptor)}'",
                    nameof(interceptorType));
            }

            var factory = new ServiceInterceptorMetadata(interceptorType);
            Add(factory);
        }
    }
}