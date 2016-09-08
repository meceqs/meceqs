using System;
using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Meceqs.Consuming
{
    public interface IFluentConsumer : IFilterContextBuilder<IFluentConsumer>
    {
        Task ConsumeAsync();

        Task<TResult> ConsumeAsync<TResult>();

        Task<object> ConsumeAsync(Type resultType);
    }
}