using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Meceqs.Filters.TypedHandling.Internal
{
    /// <summary>
    /// Uses reflection and compiled lambdas to invoke <see typeref="IHandles<,>.HandleAsync" />.
    /// Delegates are cached for better performance! This means, only the first message per type
    /// has bad performance.
    /// </summary>
    /// <remarks>See "Meceqs.Tests/Performance/TypedHandling_HandlerInvokerTest" for performance tests</remarks>
    public class DefaultHandlerInvoker : IHandlerInvoker
    {
        // CacheKey: MessageType/ResultType
        private readonly ConcurrentDictionary<Tuple<Type, Type>, Func<object, HandleContext, Task>> _cachedHandleDelegates;

        // CacheKey: ResultType
        private readonly ConcurrentDictionary<Type, Func<object, object>> _cachedTaskResultGetterDelegates;

        public DefaultHandlerInvoker()
        {
            _cachedHandleDelegates = new ConcurrentDictionary<Tuple<Type, Type>, Func<object, HandleContext, Task>>();
            _cachedTaskResultGetterDelegates = new ConcurrentDictionary<Type, Func<object, object>>();
        }

        public async Task<object> InvokeHandleAsync(object handler, HandleContext context, Type resultType)
        {
            Check.NotNull(handler, nameof(handler));
            Check.NotNull(context, nameof(context));
            Check.NotNull(resultType, nameof(resultType));

            Type messageType = context.Message.GetType();

            // Get or create delegate which invokes HandleAsync
            Func<object, HandleContext, Task> handleDelegate = GetOrAddHandleDelegate(messageType, resultType);

            // Invoke Method
            // (this throws an InvalidCastOperationException, if the handler does not have the correct types)
            Task resultTask = (Task)handleDelegate(handler, context);
            await resultTask;

            // Get or create method which returns value from task
            Func<object, object> getResultDelegate = GetOrAddResultGetterDelegate(resultType);

            // Invoke method
            object result = getResultDelegate(resultTask);

            return result;
        }

        private Func<object, HandleContext, Task> GetOrAddHandleDelegate(Type messageType, Type resultType)
        {
            var cacheKey = new Tuple<Type, Type>(messageType, resultType);

            Func<object, HandleContext, Task> handleDelegate = _cachedHandleDelegates.GetOrAdd(cacheKey, x =>
            {
                // resolve generic types
                Type typedHandlerType = typeof(IHandles<,>).MakeGenericType(x.Item1, x.Item2);

                // the MethodInfo must be created from a type which already has resolved generic types,
                // otherwise the method has unresolved generic types.
                MethodInfo typedHandleMethod = typedHandlerType.GetTypeInfo().GetDeclaredMethod("HandleAsync");

                // resolve generic types on parameters
                Type typedHandleContextType = typeof(HandleContext<>).MakeGenericType(x.Item1);

                // Create expression

                // Declaration of the object on which the method should be called
                // (cast to object is required to work with Func<object, object Task>)
                var instance = Expression.Parameter(typeof(object), "instance");

                // Declaration of the parameters that should be passed to the call
                // (cast to object is required to work with Func<object, object Task>)
                var contextArg = Expression.Parameter(typeof(object), "handleContext");

                // // Declaration of the actual method call
                var methodCall = Expression.Call(
                    Expression.Convert(instance, typedHandlerType),
                    typedHandleMethod,
                    Expression.Convert(contextArg, typedHandleContextType));

                // Compiles declaration into actual delegate
                var typedDelegate = Expression.Lambda<Func<object, HandleContext, Task>>(
                    Expression.Convert(methodCall, typeof(Task)), instance, contextArg
                ).Compile();

                return typedDelegate;
            });

            return handleDelegate;
        }

        private Func<object, object> GetOrAddResultGetterDelegate(Type resultType)
        {
            Func<object, object> getResultDelegate = _cachedTaskResultGetterDelegates.GetOrAdd(resultType, x =>
            {
                // resolve generic types
                Type taskResultType = typeof(Task<>).MakeGenericType(x);

                // MethodInfo for get-accessor
                MethodInfo getMethod = taskResultType.GetTypeInfo().GetDeclaredProperty("Result").GetMethod;

                // Create expression

                // Declaration of the object on which the method should be called
                // (cast to object is required to work with Func<object, object Task>)
                var instance = Expression.Parameter(typeof(object), "instance");

                // Declaration of the actual method call
                var methodCall = Expression.Call(
                    Expression.Convert(instance, taskResultType),
                    getMethod);

                // Compiles declaration into actual delegate
                var typedDelegate = Expression.Lambda<Func<object, object>>(methodCall, instance).Compile();

                return typedDelegate;
            });

            return getResultDelegate;
        }
    }
}