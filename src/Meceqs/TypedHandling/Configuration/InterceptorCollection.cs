using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Meceqs.TypedHandling.Configuration
{
    public class InterceptorCollection : Collection<IInterceptorMetadata>
    {
        /// <summary>
        /// Adds a type representing an <see cref="IHandleInterceptor"/>.
        /// </summary>
        /// <remarks>
        /// Interceptor instances will be created using
        /// <see cref="Microsoft.Extensions.DependencyInjection.ActivatorUtilities"/>.
        /// Use <see cref="AddService(Type)"/> to register an interceptor as a service.
        /// </remarks>
        public void Add<TInterceptor>() where TInterceptor : class, IHandleInterceptor
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
        /// Use <see cref="AddService(Type)"/> to register an interceptor as a service.
        /// </remarks>
        public void Add(Type interceptorType)
        {
            Check.NotNull(interceptorType, nameof(interceptorType));

            EnsureValidInterceptor(interceptorType);

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
        public void AddService<TInterceptor>() where TInterceptor : class, IHandleInterceptor
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

            EnsureValidInterceptor(interceptorType);

            var factory = new ServiceInterceptorMetadata(interceptorType);
            Add(factory);
        }

        public static void EnsureValidInterceptor(Type interceptorType)
        {
            if (!typeof(IHandleInterceptor).IsAssignableFrom(interceptorType))
            {
                throw new ArgumentException(
                    $"Type '{interceptorType}' must derive from '{typeof(IHandleInterceptor)}'",
                    nameof(interceptorType));
            }

            if (!interceptorType.GetTypeInfo().IsClass)
            {
                throw new ArgumentException($"Type '{interceptorType}' must be a class.", nameof(interceptorType));
            }

            if (interceptorType.GetTypeInfo().IsAbstract)
            {
                throw new ArgumentException($"Type '{interceptorType}' must not be abstract.", nameof(interceptorType));
            }
        }
    }
}