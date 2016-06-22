using System.Threading.Tasks;

namespace Meceqs.Sending.Transport
{
    public interface ISendTransport
    {
        Task<TResult> SendAsync<TMessage, TResult>(SendContext<TMessage> context)
            where TMessage : IMessage;
    }
}