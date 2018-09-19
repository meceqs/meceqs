using System;
using System.Reflection;

namespace Meceqs.TypedHandling.Internal
{
    public interface IHandleMethodResolver
    {
        MethodInfo GetHandleMethod(Type handlerType, Type messageType, Type responseType);
    }
}
