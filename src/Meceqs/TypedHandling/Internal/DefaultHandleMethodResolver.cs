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

        public MethodInfo GetHandleMethod(Type handlerType, Type messageType, Type responseType)
        {
            Guard.NotNull(handlerType, nameof(handlerType));
            Guard.NotNull(messageType, nameof(messageType));
            Guard.NotNull(responseType, nameof(responseType));

            var cacheKey = Tuple.Create(handlerType, messageType, responseType);

            var method = _methodCache.GetOrAdd(cacheKey, x =>
            {
                var handler = x.Item1;
                var message = x.Item2;
                var response = x.Item3;

                var query =
                    from mi in handler.GetTypeInfo().GetDeclaredMethods("HandleAsync")
                    where mi.ReturnType == (response == typeof(void) ? typeof(Task) : typeof(Task<>).MakeGenericType(response))
                    let parameter = mi.GetParameters().FirstOrDefault()
                    where parameter != null
                    where parameter.ParameterType == message
                    select mi;

                return query.FirstOrDefault();
            });

            return method;
        }
    }
}
