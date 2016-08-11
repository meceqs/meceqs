using System;
using System.Reflection;

namespace Meceqs.Filters.TypedHandling.Internal
{
    public interface IHandleMethodResolver
    {
        MethodInfo GetHandleMethod(Type handlerType, Type messageType, Type resultType);
    }
}