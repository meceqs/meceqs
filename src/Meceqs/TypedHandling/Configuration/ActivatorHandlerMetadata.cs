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
        private readonly Type _handlerType;
        private readonly IEnumerable<HandleDefinition> _implementedHandles;

        public IEnumerable<HandleDefinition> ImplementedHandles => _implementedHandles;

        public ActivatorHandlerMetadata(Type handlerType, IEnumerable<HandleDefinition> implementedHandles)
        {
            Check.NotNull(handlerType, nameof(handlerType));
            Check.NotNull(implementedHandles, nameof(implementedHandles));

            _handlerType = handlerType;
            _implementedHandles = implementedHandles;
        }

        public IHandles CreateHandler(IServiceProvider serviceProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            return (IHandles)ActivatorUtilities.CreateInstance(serviceProvider, _handlerType);
        }
    }
}