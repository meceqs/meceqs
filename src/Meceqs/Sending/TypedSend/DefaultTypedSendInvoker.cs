using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Meceqs.Sending.TypedSend
{
    public class DefaultTypedSendInvoker : ITypedSendInvoker
    {
        private static readonly MethodInfo _genericCreateSenderMethod = typeof(ISenderFactory).GetTypeInfo().GetDeclaredMethod(nameof(ISenderFactory.CreateSender));

        private readonly ConcurrentDictionary<Tuple<Type, Type>, Func<ISenderFactory, object>> _createSenderDelegateCache;

        public DefaultTypedSendInvoker()
        {
            _createSenderDelegateCache = new ConcurrentDictionary<Tuple<Type, Type>, Func<ISenderFactory, object>>();
        }

        public object InvokeCreateSender(ISenderFactory senderFactory, Type messageType, Type resultType)
        {
            Check.NotNull(senderFactory, nameof(senderFactory));
            Check.NotNull(messageType, nameof(messageType));
            Check.NotNull(resultType, nameof(resultType));

            var cacheKey = new Tuple<Type, Type>(messageType, resultType);

            Func<ISenderFactory, object> createSenderDelegate = _createSenderDelegateCache.GetOrAdd(cacheKey, x =>
            {
                // Reflection magic for better performance!
                // We create a compiled delegate of the `CreateSender<TMessage, TResult>` method
                // and cache it. This means, only the first message has bad performance.
                //  
                // https://vagifabilov.wordpress.com/2010/04/02/dont-use-activator-createinstance-or-constructorinfo-invoke-use-compiled-lambda-expressions/
                // http://snipplr.com/view/29683/converting-methodinfo-into-a-delegate-instance-to-improve-performance/
                // https://codeblog.jonskeet.uk/2008/08/09/making-reflection-fly-and-exploring-delegates/
                // http://www.marccostello.com/newing-up-t/

                // The untyped CreateSender-method must be converted to a message-specific typed version
                MethodInfo typedCreateSenderMethod = _genericCreateSenderMethod.MakeGenericMethod(messageType, resultType);

                // Declaration of the object on which the method should be called
                var instance = Expression.Parameter(typeof(ISenderFactory), "instance");

                // Declaration of the actual method call
                var methodCall = Expression.Call(instance, typedCreateSenderMethod);

                // Compiles declaration into actual delegate
                Func<ISenderFactory, object> typedDelegate = Expression.Lambda<Func<ISenderFactory, object>>(
                    Expression.Convert(methodCall, typeof(object)), instance
                ).Compile();

                return typedDelegate;
            });

            object sender = createSenderDelegate(senderFactory);

            return sender;
        }

        public Task<TResult> InvokeSendAsync<TResult>(object sender, MessageContext context)
        {
            Check.NotNull(sender, nameof(sender));
            Check.NotNull(context, nameof(context));

            //MethodInfo sendMethod = typeof(ISender<,>).GetTypeInfo().GetDeclaredMethod(nameof(ISender<IMessage, VoidType>.SendAsync));

            //return (Task<TResult>)sendMethod.Invoke(sender, new[] { context });

            dynamic dynamicSender = (dynamic)sender;

            return (Task<TResult>)dynamicSender.SendAsync(context);
        }
    }
}