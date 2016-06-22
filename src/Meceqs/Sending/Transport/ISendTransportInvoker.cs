using System.Threading.Tasks;

namespace Meceqs.Sending.Transport
{
    public interface ISendTransportInvoker
    {
        Task<TResult> InvokeSendAsync<TMessage, TResult>(ISendTransport transport, SendContext<TMessage> context)
            where TMessage : IMessage;
    }
}