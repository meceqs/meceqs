using System.Threading.Tasks;

namespace Meceqs.Sending
{
    public interface ISendTransportInvoker
    {
        Task<TResult> InvokeSendAsync<TResult>(ISendTransport transport, MessageContext context);
    }
}