using System;

namespace Meceqs.Filters.TypedHandling.Internal
{
    public interface IHandlerFactoryInvoker
    {
        IHandles InvokeCreateHandler(IHandlerFactory handlerFactory, Type messageType, Type resultType);
    }
}