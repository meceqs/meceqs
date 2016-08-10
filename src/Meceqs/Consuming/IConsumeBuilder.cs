using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Consuming
{
    public interface IConsumeBuilder
    {
        IConsumeBuilder SetCancellationToken(CancellationToken cancellation);

        IConsumeBuilder SetContextItem(string key, object value);

        Task ConsumeAsync();

        Task<TResult> ConsumeAsync<TResult>();
    }
}