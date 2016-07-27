using System;
using System.Threading.Tasks;

namespace Meceqs.Sending.TypedSend
{
    public interface ITypedSendInvoker
    {
        object InvokeCreateSender(ISenderFactory senderFactory, Type messageType, Type resultType);

        Task<TResult> InvokeSendAsync<TResult>(object sender, MessageContext context);
    }
}