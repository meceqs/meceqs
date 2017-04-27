using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.TypedHandling.Configuration
{
    /// <summary>
    /// Resolves a <see cref="IHandles" /> using <see cref="ActivatorUtilities" />.
    /// </summary>
    public class ActivatorHandlerMetadata : IHandlerMetadata
    {
        public Type HandlerType { get; }

        public IEnumerable<HandleDefinition> ImplementedHandles { get; }

        public ActivatorHandlerMetadata(Type handlerType, IEnumerable<HandleDefinition> implementedHandles)
        {
            Check.NotNull(handlerType, nameof(handlerType));
            Check.NotNull(implementedHandles, nameof(implementedHandles));

            HandlerType = handlerType;
            ImplementedHandles = implementedHandles;
        }

        public IHandles CreateHandler(IServiceProvider serviceProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            return (IHandles)ActivatorUtilities.CreateInstance(serviceProvider, HandlerType);
        }
    }
}