using System;
using System.Threading.Tasks;

namespace Meceqs.Sending.TypedSend
{
    public class TypedSendTransport : ISendTransport
    {
        private readonly ISenderFactory _senderFactory;
        private readonly ISenderFactoryInvoker _senderFactoryInvoker;
        private readonly ISenderInvoker _senderInvoker;


        public TypedSendTransport(ISenderFactory senderFactory, ISenderFactoryInvoker senderFactoryInvoker, ISenderInvoker senderInvoker)
        {
            Check.NotNull(senderFactory, nameof(senderFactory));
            Check.NotNull(senderFactoryInvoker, nameof(senderFactoryInvoker));
            Check.NotNull(senderInvoker, nameof(senderInvoker));

            _senderFactory = senderFactory;
            _senderFactoryInvoker = senderFactoryInvoker;
            _senderInvoker = senderInvoker;
        }

        public Task<TResult> SendAsync<TResult>(MessageContext context)
        {
            Check.NotNull(context, nameof(context));

            // ISenderFactory and ISender expect generic types so we have to use reflection.
            // The calls are outsourced to separate invokers to make sure that they 
            // can be optimized independently.

            Type messageType = context.Message.GetType();
            Type resultType = typeof(TResult);

            object sender = _senderFactoryInvoker.InvokeCreateSender(_senderFactory, messageType, resultType);

            if (sender == null)
            {
                throw new InvalidOperationException($"No sender found for '{messageType.Name}/{resultType.Name}'");
            }

            return _senderInvoker.InvokeSendAsync<TResult>(sender, context);
        }
    }
}