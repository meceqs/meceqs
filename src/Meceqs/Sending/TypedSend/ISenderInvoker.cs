using System.Threading.Tasks;

namespace Meceqs.Sending.TypedSend
{
    public interface ISenderInvoker
    {
        Task<TResult> InvokeSendAsync<TResult>(object sender, MessageContext context);
    }
}