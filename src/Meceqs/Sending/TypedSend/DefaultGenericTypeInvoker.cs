using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Meceqs.Sending.TypedSend
{
    public class DefaultTypedSendInvoker : ITypedSendInvoker
    {
        public object InvokeCreateSender(ISenderFactory senderFactory, Type messageType, Type resultType)
        {
            Check.NotNull(senderFactory, nameof(senderFactory));
            Check.NotNull(messageType, nameof(messageType));
            Check.NotNull(resultType, nameof(resultType));

            // TODO @cweiss Caching!!!!
            MethodInfo genericCreateSenderMethod = typeof(ISenderFactory).GetTypeInfo().GetDeclaredMethod(nameof(ISenderFactory.CreateSender));
            MethodInfo typedCreateSenderMethod = genericCreateSenderMethod.MakeGenericMethod(messageType, resultType);

            object sender = typedCreateSenderMethod.Invoke(senderFactory, null);

            return sender;
        }

        public Task<TResult> InvokeSendAsync<TResult>(object sender, MessageContext context)
        {
            Check.NotNull(sender, nameof(sender));
            Check.NotNull(context, nameof(context));

            MethodInfo sendMethod = typeof(ISender<,>).GetTypeInfo().GetDeclaredMethod(nameof(ISender<IMessage, VoidType>.SendAsync));

            return (Task<TResult>)sendMethod.Invoke(sender, new [] { context });
        }
    }
}