using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Meceqs.Filters.TypedHandling.Internal
{
    public class DefaultHandlerFactoryInvoker : IHandlerFactoryInvoker
    {
        private static readonly MethodInfo _genericCreateHandlerMethod = typeof(IHandlerFactory).GetTypeInfo().GetDeclaredMethod(nameof(IHandlerFactory.CreateHandler));

        private readonly ConcurrentDictionary<Tuple<Type, Type>, Func<IHandlerFactory, object>> _createHandlerDelegateCache;

        public DefaultHandlerFactoryInvoker()
        {
            _createHandlerDelegateCache = new ConcurrentDictionary<Tuple<Type, Type>, Func<IHandlerFactory, object>>();
        }

        public object InvokeCreateHandler(IHandlerFactory handlerFactory, Type messageType, Type resultType)
        {
            Check.NotNull(handlerFactory, nameof(handlerFactory));
            Check.NotNull(messageType, nameof(messageType));
            Check.NotNull(resultType, nameof(resultType));

            Func<IHandlerFactory, object> createHandlerDelegate = GetOrAddCreateHandlerDelegate(messageType, resultType);

            object handler = createHandlerDelegate(handlerFactory);

            return handler;
        }

        private Func<IHandlerFactory, object> GetOrAddCreateHandlerDelegate(Type messageType, Type resultType)
        {
            var cacheKey = new Tuple<Type, Type>(messageType, resultType);

            Func<IHandlerFactory, object> createHandlerDelegate = _createHandlerDelegateCache.GetOrAdd(cacheKey, x =>
            {
                // Reflection magic for better performance!
                // We create a compiled delegate of the `CreateHandler<TMessage, TResult>` method
                // and cache it. This means, only the first message has bad performance.
                //  
                // https://vagifabilov.wordpress.com/2010/04/02/dont-use-activator-createinstance-or-constructorinfo-invoke-use-compiled-lambda-expressions/
                // http://snipplr.com/view/29683/converting-methodinfo-into-a-delegate-instance-to-improve-performance/
                // https://codeblog.jonskeet.uk/2008/08/09/making-reflection-fly-and-exploring-delegates/
                // http://www.marccostello.com/newing-up-t/

                // The untyped CreateSender-method must be converted to a message-specific typed version
                MethodInfo typedCreateHandlerMethod = _genericCreateHandlerMethod.MakeGenericMethod(x.Item1, x.Item2);

                // Declaration of the object on which the method should be called
                var instance = Expression.Parameter(typeof(IHandlerFactory), "instance");

                // Declaration of the actual method call
                var methodCall = Expression.Call(instance, typedCreateHandlerMethod);

                // Compiles declaration into actual delegate
                Func<IHandlerFactory, object> typedDelegate = Expression.Lambda<Func<IHandlerFactory, object>>(
                    Expression.Convert(methodCall, typeof(object)), instance
                ).Compile();

                return typedDelegate;
            });

            return createHandlerDelegate;
        }
    }
}