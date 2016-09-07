using System;
using System.Collections.Generic;

namespace Meceqs.Filters.TypedHandling.Configuration
{
    public interface IHandlerMetadata
    {
        IEnumerable<Tuple<Type, Type>> ImplementedHandles { get; }

        IHandles CreateHandler(IServiceProvider serviceProvider);
    }
}