using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Consuming
{
    public interface IFluentConsumer
    {
        IFluentConsumer SetCancellationToken(CancellationToken cancellation);

        IFluentConsumer SetContextItem(string key, object value);

        IFluentConsumer UsePipeline(string pipelineName);

        Task ConsumeAsync();

        Task<TResult> ConsumeAsync<TResult>();
    }
}