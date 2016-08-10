using System;

namespace Meceqs.Filters.TypedHandling.Internal
{
    public interface IHandlerFactoryInvoker
    {
        object InvokeCreateHandler(IHandlerFactory handlerFactory, Type messageType, Type resultType);
    }
}