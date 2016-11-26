using System;
using System.Collections.Generic;

namespace Meceqs.TypedHandling.Configuration
{
    /// <summary>
    /// Resolves a <see cref="IHandles" /> using <see cref="IServiceProvider" />.
    /// </summary>
    public class ServiceHandlerMetadata : IHandlerMetadata
    {
        private readonly Type _handlerType;

        public IEnumerable<HandleDefinition> ImplementedHandles { get; }

        public ServiceHandlerMetadata(Type handlerType, IEnumerable<HandleDefinition> implementedHandles)
        {
            Check.NotNull(handlerType, nameof(handlerType));
            Check.NotNull(implementedHandles, nameof(implementedHandles));

            _handlerType = handlerType;
            ImplementedHandles = implementedHandles;
        }

        public IHandles CreateHandler(IServiceProvider serviceProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            // This doesn't use "GetRequiredService" because it's up to the middleware
            // to decide whether unknown messages should throw or not.
            return (IHandles)serviceProvider.GetService(_handlerType);
        }
    }
}