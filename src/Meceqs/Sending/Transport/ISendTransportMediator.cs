using System.Threading.Tasks;

namespace Meceqs.Sending.Transport
{
    public interface ISendTransportMediator
    {
        Task<TResult> SendAsync<TMessage, TResult>(SendContext<TMessage> context) where TMessage : IMessage;
    }
}