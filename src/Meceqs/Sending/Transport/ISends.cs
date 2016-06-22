using System.Threading.Tasks;

namespace Meceqs.Sending.TypedSend
{
    public interface ISends<TMessage, TResult> where TMessage : IMessage
    {
        Task<TResult> SendAsync(SendContext<TMessage> context);
    }
}