using System.Threading.Tasks;

namespace Meceqs.Sending.Transport.TypedSend
{
    public interface ISender<TMessage, TResult> where TMessage : IMessage
    {
        Task<TResult> SendAsync(MessageContext<TMessage> context);
    }
}