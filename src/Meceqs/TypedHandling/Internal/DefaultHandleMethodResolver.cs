using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Meceqs.TypedHandling.Internal
{
    public class DefaultHandleMethodResolver : IHandleMethodResolver
    {
        private readonly ConcurrentDictionary<Tuple<Type, Type, Type>, MethodInfo> _methodCache;

        public DefaultHandleMethodResolver()
        {
            _methodCache = new ConcurrentDictionary<Tuple<Type, Type, Type>, MethodInfo>();
        }

        public MethodInfo GetHandleMethod(Type handlerType, Type messageType, Type resultType)
        {
            Check.NotNull(handlerType, nameof(handlerType));
            Check.NotNull(messageType, nameof(messageType));

            var cacheKey = new Tuple<Type, Type, Type>(handlerType, messageType, resultType);

            var method = _methodCache.GetOrAdd(cacheKey, x =>
            {
                var handler = x.Item1;
                var message = x.Item2;
                var result = x.Item3;

                var query =
                    from mi in handler.GetTypeInfo().GetDeclaredMethods("HandleAsync")
                    where mi.ReturnType == (result == null ? typeof(Task) : typeof(Task<>).MakeGenericType(result))
                    let parameter = mi.GetParameters().FirstOrDefault()
                    where parameter != null
                    where parameter.ParameterType == typeof(HandleContext<>).MakeGenericType(message)
                    select mi;

                return query.FirstOrDefault();
            });

            return method;
        }
    }
}