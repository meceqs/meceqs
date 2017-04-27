using System;
using System.Collections.Generic;

namespace Meceqs.TypedHandling.Configuration
{
    public interface IHandlerMetadata
    {
        Type HandlerType { get; }

        IEnumerable<HandleDefinition> ImplementedHandles { get; }

        IHandles CreateHandler(IServiceProvider serviceProvider);
    }
}