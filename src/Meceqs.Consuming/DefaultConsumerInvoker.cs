using System.Threading.Tasks;

namespace Meceqs.Consuming
{
    public class DefaultConsumerInvoker : IConsumerInvoker
    {
        public async Task<TResult> InvokeAsync<TMessage, TResult>(IConsumes<TMessage, TResult> consumer, ConsumeContext<TMessage> context)
            where TMessage : IMessage
        {
            return await consumer.ConsumeAsync(context);
        }
    }
}