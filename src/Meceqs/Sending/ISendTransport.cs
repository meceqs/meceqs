using System.Threading.Tasks;

namespace Meceqs.Sending
{
    public interface ISendTransport
    {
        Task<TResult> SendAsync<TResult>(MessageContext context);
    }
}