using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Meceqs.TypedHandling.Internal
{
    /// <summary>
    /// Uses reflection and compiled lambdas to invoke either <see typeref="IHandles{,}.HandleAsync" />
    /// or <see typeref="IHandles{}.HandleAsync" />.
    /// Delegates are cached for better performance! This means, only the first message per type
    /// has bad performance.
    /// </summary>
    /// <remarks>See "Meceqs.Tests/Performance/TypedHandling_HandlerInvokerTest" for performance tests</remarks>
    public class DefaultHandlerInvoker : IHandlerInvoker
    {
        // CacheKey: MessageType/ResponseType
        private readonly ConcurrentDictionary<Tuple<Type, Type>, Func<IHandles, object, HandleContext, Task>> _cachedHandleDelegates;

        // CacheKey: ResponseType
        private readonly ConcurrentDictionary<Type, Func<Task, object>> _cachedTaskResultGetterDelegates;

        public DefaultHandlerInvoker()
        {
            _cachedHandleDelegates = new ConcurrentDictionary<Tuple<Type, Type>, Func<IHandles, object, HandleContext, Task>>();
            _cachedTaskResultGetterDelegates = new ConcurrentDictionary<Type, Func<Task, object>>();
        }

        public async Task InvokeHandleAsync(HandleContext context)
        {
            Guard.NotNull(context, nameof(context));

            Type messageType = context.Message.GetType();
            Type responseType = context.MessageContext.ExpectedResponseType;

            Func<IHandles, object, HandleContext, Task> handleDelegate = GetOrAddHandleDelegate(messageType, responseType);

            // Invoke Method
            // (this throws an InvalidCastOperationException, if the handler does not have the correct types)
            Task responseTask = handleDelegate(context.Handler, context.Message, context);

            await responseTask;

            if (responseType != typeof(void))
            {
                Func<Task, object> getTaskResultDelegate = GetOrAddTaskResultGetterDelegate(responseType);

                context.MessageContext.Response = getTaskResultDelegate(responseTask);
            }
        }

        private Func<IHandles, object, HandleContext, Task> GetOrAddHandleDelegate(Type messageType, Type responseType)
        {
            var cacheKey = Tuple.Create(messageType, responseType);

            Func<IHandles, object, HandleContext, Task> handleDelegate = _cachedHandleDelegates.GetOrAdd(cacheKey, x =>
            {
                // resolve correct type based on whether there should be a response.
                Type typedHandlerType = x.Item2 != typeof(void)
                    ? typeof(IHandles<,>).MakeGenericType(x.Item1, x.Item2)
                    : typeof(IHandles<>).MakeGenericType(x.Item1);

                // the MethodInfo must be created from a type which already has resolved generic types,
                // otherwise the method has unresolved generic types.
                MethodInfo typedHandleMethod = typedHandlerType.GetTypeInfo().GetDeclaredMethod("HandleAsync");

                // Create expression

                // Declaration of the object on which the method should be called
                var instance = Expression.Parameter(typeof(IHandles), "instance");

                // Declaration of the parameters that should be passed to the call
                var messageArg = Expression.Parameter(typeof(object), "message");
                var contextArg = Expression.Parameter(typeof(HandleContext), "handleContext");

                // // Declaration of the actual method call
                var methodCall = Expression.Call(
                    Expression.Convert(instance, typedHandlerType),
                    typedHandleMethod,
                    Expression.Convert(messageArg, messageType),
                    contextArg);

                // Compiles declaration into actual delegate
                var typedDelegate = Expression.Lambda<Func<IHandles, object, HandleContext, Task>>(
                    Expression.Convert(methodCall, typeof(Task)), instance, messageArg, contextArg
                ).Compile();

                return typedDelegate;
            });

            return handleDelegate;
        }

        private Func<Task, object> GetOrAddTaskResultGetterDelegate(Type resultType)
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
