using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Meceqs.Sending.TypedSend
{
    /// <summary>
    /// Uses reflection and compiled lambdas to invoke <see typeref="ISender<,>.SendAsync" />.
    /// Delegates are cached for better performance! This means, only the first message per type
    /// has bad performance.
    /// </summary>
    /// <remarks>See "Meceqs.Tests/Performance/TypedSend_SenderInvokerTest" for performance tests</remarks>
    public class DefaultSenderInvoker : ISenderInvoker
    {
        // CacheKey: MessageType/ResultType
        private readonly ConcurrentDictionary<Tuple<Type, Type>, Func<object, object, Task>> _cachedSendDelegates;

        // CacheKey: ResultType
        private readonly ConcurrentDictionary<Type, Func<object, object>> _cachedTaskResultGetterDelegates;

        public DefaultSenderInvoker()
        {
            _cachedSendDelegates = new ConcurrentDictionary<Tuple<Type, Type>, Func<object, object, Task>>();
            _cachedTaskResultGetterDelegates = new ConcurrentDictionary<Type, Func<object, object>>();
        }

        public async Task<TResult> InvokeSendAsync<TResult>(object sender, MessageContext context)
        {
            Check.NotNull(sender, nameof(sender));
            Check.NotNull(context, nameof(context));

            Type messageType = context.Message.GetType();
            Type resultType = typeof(TResult);

            // Get or create delegate which invokes SendAsync
            Func<object, object, Task> sendDelegate = GetOrAddSendDelegate(messageType, resultType);

            // Invoke Method
            // (this throws an InvalidCastOperationException, if the sender does not have the correct types)
            Task resultTask = (Task)sendDelegate(sender, context);
            await resultTask;

            // Get or create method which returns value from task
            Func<object, object> getResultDelegate = GetOrAddResultGetterDelegate(resultType);

            // Invoke method
            TResult result = (TResult)getResultDelegate(resultTask);

            return result;
        }

        private Func<object, object, Task> GetOrAddSendDelegate(Type messageType, Type resultType)
        {
            var cacheKey = new Tuple<Type, Type>(messageType, resultType);

            Func<object, object, Task> sendDelegate = _cachedSendDelegates.GetOrAdd(cacheKey, x =>
            {
                // resolve generic types on ISender
                Type typedSenderType = typeof(ISender<,>).MakeGenericType(x.Item1, x.Item2);

                // the MethodInfo must be created from a type which already has resolved generic types,
                // otherwise the method has unresolved generic types.
                MethodInfo typedSendMethod = typedSenderType.GetTypeInfo().GetDeclaredMethod("SendAsync");

                // resolve generic types on parameters of SendAsync method
                Type typedMessageContextType = typeof(MessageContext<>).MakeGenericType(x.Item1);

                // Create expression

                // Declaration of the object on which the method should be called
                // (cast to object is required to work with Func<object, object Task>)
                var instance = Expression.Parameter(typeof(object), "instance");

                // Declaration of the parameters that should be passed to the call
                // (cast to object is required to work with Func<object, object Task>)
                var contextArg = Expression.Parameter(typeof(object), "messageContext");

                // // Declaration of the actual method call
                var methodCall = Expression.Call(
                    Expression.Convert(instance, typedSenderType),
                    typedSendMethod,
                    Expression.Convert(contextArg, typedMessageContextType));

                // Compiles declaration into actual delegate
                var typedDelegate = Expression.Lambda<Func<object, object, Task>>(
                    Expression.Convert(methodCall, typeof(Task)), instance, contextArg
                ).Compile();

                return typedDelegate;
            });

            return sendDelegate;
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