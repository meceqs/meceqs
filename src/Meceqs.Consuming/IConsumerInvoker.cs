using System.Threading.Tasks;

namespace Meceqs.Consuming
{
    public interface IConsumerInvoker
    {
        Task<TResult> InvokeAsync<TMessage, TResult>(IConsumes<TMessage, TResult> consumer, ConsumeContext<TMessage> context)
            where TMessage : IMessage;
    }
}