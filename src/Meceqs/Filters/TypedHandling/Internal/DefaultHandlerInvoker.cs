using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Meceqs.Filters.TypedHandling.Internal
{
    /// <summary>
    /// Uses reflection and compiled lambdas to invoke either <see typeref="IHandles<,>.HandleAsync" />
    /// or <see typeref="IHandles<>.HandleAsync" />.
    /// Delegates are cached for better performance! This means, only the first message per type
    /// has bad performance.
    /// </summary>
    /// <remarks>See "Meceqs.Tests/Performance/TypedHandling_HandlerInvokerTest" for performance tests</remarks>
    public class DefaultHandlerInvoker : IHandlerInvoker
    {
        // CacheKey: MessageType/ResultType
        private readonly ConcurrentDictionary<Tuple<Type, Type>, Func<IHandles, HandleContext, Task>> _cachedHandleDelegates;

        // CacheKey: ResultType
        private readonly ConcurrentDictionary<Type, Func<Task, object>> _cachedTaskResultGetterDelegates;

        public DefaultHandlerInvoker()
        {
            _cachedHandleDelegates = new ConcurrentDictionary<Tuple<Type, Type>, Func<IHandles, HandleContext, Task>>();
            _cachedTaskResultGetterDelegates = new ConcurrentDictionary<Type, Func<Task, object>>();
        }

        public async Task<object> InvokeHandleAsync(IHandles handler, HandleContext context, Type resultType)
        {
            Check.NotNull(handler, nameof(handler));
            Check.NotNull(context, nameof(context));

            Type messageType = context.Message.GetType();

            Func<IHandles, HandleContext, Task> handleDelegate = GetOrAddHandleDelegate(messageType, resultType);

            // Invoke Method
            // (this throws an InvalidCastOperationException, if the handler does not have the correct types)
            Task resultTask = handleDelegate(handler, context);
            await resultTask;

            if (resultType == null)
            {
                return null;
            }
            else
            {
                Func<Task, object> getResultDelegate = GetOrAddResultGetterDelegate(resultType);
                
                object result = getResultDelegate(resultTask);

                return result;
            }
        }

        private Func<IHandles, HandleContext, Task> GetOrAddHandleDelegate(Type messageType, Type resultType)
        {
            var cacheKey = new Tuple<Type, Type>(messageType, resultType);

            Func<IHandles, HandleContext, Task> handleDelegate = _cachedHandleDelegates.GetOrAdd(cacheKey, x =>
            {
                // resolve correct type based on whether there should be a result.
                Type typedHandlerType = x.Item2 != null
                    ? typeof(IHandles<,>).MakeGenericType(x.Item1, x.Item2)
                    : typeof(IHandles<>).MakeGenericType(x.Item1);

                // the MethodInfo must be created from a type which already has resolved generic types,
                // otherwise the method has unresolved generic types.
                MethodInfo typedHandleMethod = typedHandlerType.GetTypeInfo().GetDeclaredMethod("HandleAsync");

                // resolve generic types on parameters
                Type typedHandleContextType = typeof(HandleContext<>).MakeGenericType(x.Item1);

                // Create expression

                // Declaration of the object on which the method should be called
                var instance = Expression.Parameter(typeof(IHandles), "instance");

                // Declaration of the parameters that should be passed to the call
                var contextArg = Expression.Parameter(typeof(HandleContext), "handleContext");

                // // Declaration of the actual method call
                var methodCall = Expression.Call(
                    Expression.Convert(instance, typedHandlerType),
                    typedHandleMethod,
                    Expression.Convert(contextArg, typedHandleContextType));

                // Compiles declaration into actual delegate
                var typedDelegate = Expression.Lambda<Func<IHandles, HandleContext, Task>>(
                    Expression.Convert(methodCall, typeof(Task)), instance, contextArg
                ).Compile();

                return typedDelegate;
            });

            return handleDelegate;
        }

        private Func<Task, object> GetOrAddResultGetterDelegate(Type resultType)
        {
            Func<Task, object> getResultDelegate = _cachedTaskResultGetterDelegates.GetOrAdd(resultType, x =>
            {
                // resolve generic types
                Type taskResultType = typeof(Task<>).MakeGenericType(x);

                // MethodInfo for get-accessor
                MethodInfo getMethod = taskResultType.GetTypeInfo().GetDeclaredProperty("Result").GetMethod;

                // Create expression

                var instance = Expression.Parameter(typeof(Task), "instance");
                var getCall = Expression.Call(Expression.Convert(instance, taskResultType), getMethod);
                var castedResult = Expression.Convert(getCall, typeof(object));

                // Compiles declaration into actual delegate
                var typedDelegate = Expression.Lambda<Func<Task, object>>(castedResult, instance).Compile();

                return typedDelegate;
            });

            return getResultDelegate;
        }
    }
}