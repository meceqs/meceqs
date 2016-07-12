using System.Threading.Tasks;

namespace Meceqs.Handling
{
    public interface IHandler<TMessage, TResult> where TMessage : IMessage
    {
        Task<TResult> HandleAsync(MessageContext<TMessage> context);
    }
}