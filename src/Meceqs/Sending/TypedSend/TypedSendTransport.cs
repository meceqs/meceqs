using System;
using System.Threading.Tasks;

namespace Meceqs.Sending.TypedSend
{
    public class TypedSendTransport : ISendTransport
    {
        private readonly ITypedSendInvoker _typedSendInvoker;
        private readonly ISenderFactory _senderFactory;

        public TypedSendTransport(ITypedSendInvoker typedSendInvoker, ISenderFactory senderFactory)
        {
            Check.NotNull(typedSendInvoker, nameof(typedSendInvoker));
            Check.NotNull(senderFactory, nameof(senderFactory));

            _typedSendInvoker = typedSendInvoker;
            _senderFactory = senderFactory;
        }

        public Task<TResult> SendAsync<TResult>(MessageContext context)
        {
            Check.NotNull(context, nameof(context));

            // ISenderFactory and ISender expect generic types so we have to use reflection.
            // The calls are outsourced to a separate invoker to make sure that it 
            // can be optimized independently.

            Type messageType = context.Message.GetType();
            Type resultType = typeof(TResult);

            object sender = _typedSendInvoker.InvokeCreateSender(_senderFactory, messageType, resultType);
            
            if (sender == null)
            {
                throw new InvalidOperationException($"No sender found for '{messageType.Name}/{resultType.Name}'");
            }

            return _typedSendInvoker.InvokeSendAsync<TResult>(sender, context);
        }
    }
}