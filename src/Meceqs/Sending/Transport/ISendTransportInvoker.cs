using System.Threading.Tasks;

namespace Meceqs.Sending.Transport
{
    public interface ISendTransportInvoker
    {
        Task<TResult> InvokeSendAsync<TMessage, TResult>(ISendTransport transport, MessageContext<TMessage> context)
            where TMessage : IMessage;
    }
}