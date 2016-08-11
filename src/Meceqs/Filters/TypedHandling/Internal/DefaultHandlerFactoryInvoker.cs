using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Meceqs.Filters.TypedHandling.Internal
{
    public class DefaultHandlerFactoryInvoker : IHandlerFactoryInvoker
    {
        private static readonly MethodInfo _createHandlerMethodWithoutResult = typeof(IHandlerFactory).GetTypeInfo()
            .GetDeclaredMethods(nameof(IHandlerFactory.CreateHandler))
            .First(x => x.GetGenericArguments().Count() == 1);

        private static readonly MethodInfo _createHandlerMethodWithResult = typeof(IHandlerFactory).GetTypeInfo()
            .GetDeclaredMethods(nameof(IHandlerFactory.CreateHandler))
            .First(x => x.GetGenericArguments().Count() == 2);

        private readonly ConcurrentDictionary<Tuple<Type, Type>, Func<IHandlerFactory, IHandles>> _createHandlerDelegateCache;

        public DefaultHandlerFactoryInvoker()
        {
            _createHandlerDelegateCache = new ConcurrentDictionary<Tuple<Type, Type>, Func<IHandlerFactory, IHandles>>();
        }

        public IHandles InvokeCreateHandler(IHandlerFactory handlerFactory, Type messageType, Type resultType)
        {
            Check.NotNull(handlerFactory, nameof(handlerFactory));
            Check.NotNull(messageType, nameof(messageType));

            Func<IHandlerFactory, IHandles> createHandlerDelegate = GetOrAddCreateHandlerDelegate(messageType, resultType);

            IHandles handler = createHandlerDelegate(handlerFactory);

            return handler;
        }

        private Func<IHandlerFactory, IHandles> GetOrAddCreateHandlerDelegate(Type messageType, Type resultType)
        {
            var cacheKey = new Tuple<Type, Type>(messageType, resultType);

            Func<IHandlerFactory, IHandles> createHandlerDelegate = _createHandlerDelegateCache.GetOrAdd(cacheKey, x =>
            {
                // Reflection magic for better performance!
                // We create a compiled delegate of either `CreateHandler<TMessage>` or `CreateHandler<TMessage, TResult>` method
                // and cache it. This means, only the first message per type has bad performance.
                // 
                // https://vagifabilov.wordpress.com/2010/04/02/dont-use-activator-createinstance-or-constructorinfo-invoke-use-compiled-lambda-expressions/
                // http://snipplr.com/view/29683/converting-methodinfo-into-a-delegate-instance-to-improve-performance/
                // https://codeblog.jonskeet.uk/2008/08/09/making-reflection-fly-and-exploring-delegates/
                // http://www.marccostello.com/newing-up-t/

                // choose the correct typed method based on whether there should be a result object.
                MethodInfo typedCreateHandlerMethod = x.Item2 != null
                    ? _createHandlerMethodWithResult.MakeGenericMethod(x.Item1, x.Item2)
                    : _createHandlerMethodWithoutResult.MakeGenericMethod(x.Item1);

                // Declaration of the object on which the method should be called
                var instance = Expression.Parameter(typeof(IHandlerFactory), "instance");

                // Declaration of the actual method call
                var methodCall = Expression.Call(instance, typedCreateHandlerMethod);

                // Compiles declaration into actual delegate
                Func<IHandlerFactory, IHandles> typedDelegate = Expression.Lambda<Func<IHandlerFactory, IHandles>>(
                    Expression.Convert(methodCall, typeof(IHandles)), instance
                ).Compile();

                return typedDelegate;
            });

            return createHandlerDelegate;
        }
    }
}