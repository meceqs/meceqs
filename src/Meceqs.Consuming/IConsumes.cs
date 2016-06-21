using System.Threading.Tasks;

namespace Meceqs.Consuming
{
    public interface IConsumes<TMessage, TResult> where TMessage : IMessage
    {
        Task<TResult> ConsumeAsync(ConsumeContext<TMessage> context);
    }
}