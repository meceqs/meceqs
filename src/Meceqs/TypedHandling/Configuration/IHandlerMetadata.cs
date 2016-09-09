using System;
using System.Collections.Generic;

namespace Meceqs.TypedHandling.Configuration
{
    public interface IHandlerMetadata
    {
        IEnumerable<HandleDefinition> ImplementedHandles { get; }

        IHandles CreateHandler(IServiceProvider serviceProvider);
    }
}