using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Filters.TypedHandling.Configuration
{
    /// <summary>
    /// Resolves a <see cref="IHandles" /> using <see cref="IServiceProvider" />.
    /// </summary>
    public class ServiceHandlerMetadata : IHandlerMetadata
    {
        private readonly Type _handlerType;

        public IEnumerable<Tuple<Type, Type>> ImplementedHandles { get; }

        public ServiceHandlerMetadata(Type handlerType, IEnumerable<Tuple<Type, Type>> implementedHandles)
        {
            Check.NotNull(handlerType, nameof(handlerType));
            Check.NotNull(implementedHandles, nameof(implementedHandles));

            _handlerType = handlerType;
            ImplementedHandles = implementedHandles;
        }

        public IHandles CreateHandler(IServiceProvider serviceProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            return (IHandles)serviceProvider.GetRequiredService(_handlerType);
        }
    }
}