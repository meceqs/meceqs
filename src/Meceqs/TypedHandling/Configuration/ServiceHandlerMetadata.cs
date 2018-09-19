using System;
using System.Collections.Generic;

namespace Meceqs.TypedHandling.Configuration
{
    /// <summary>
    /// Resolves a <see cref="IHandles" /> using <see cref="IServiceProvider" />.
    /// </summary>
    public class ServiceHandlerMetadata : IHandlerMetadata
    {
        public Type HandlerType { get; }

        public IEnumerable<HandleDefinition> ImplementedHandles { get; }

        public ServiceHandlerMetadata(Type handlerType, IEnumerable<HandleDefinition> implementedHandles)
        {
            Guard.NotNull(handlerType, nameof(handlerType));
            Guard.NotNull(implementedHandles, nameof(implementedHandles));

            HandlerType = handlerType;
            ImplementedHandles = implementedHandles;
        }

        public IHandles CreateHandler(IServiceProvider serviceProvider)
        {
            Guard.NotNull(serviceProvider, nameof(serviceProvider));

            // This doesn't use "GetRequiredService" because it's up to the middleware
            // to decide whether unknown messages should throw or not.
            return (IHandles)serviceProvider.GetService(HandlerType);
        }
    }
}
