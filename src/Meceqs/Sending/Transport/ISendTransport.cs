using System.Threading.Tasks;

namespace Meceqs.Sending.Transport
{
    public interface ISendTransport
    {
        Task<TResult> SendAsync<TMessage, TResult>(MessageContext<TMessage> context)
            where TMessage : IMessage;
    }
}