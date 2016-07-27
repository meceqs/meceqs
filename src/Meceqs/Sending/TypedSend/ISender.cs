using System.Threading.Tasks;

namespace Meceqs.Sending.TypedSend
{
    public interface ISender<TMessage, TResult> where TMessage : IMessage
    {
        Task<TResult> SendAsync(MessageContext<TMessage> context);
    }
}