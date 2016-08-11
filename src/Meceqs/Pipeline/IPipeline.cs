using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meceqs.Pipeline
{
    public interface IPipeline
    {
        Task ProcessAsync(FilterContext context);

        Task ProcessAsync(IList<FilterContext> contexts);

        Task<TResult> ProcessAsync<TResult>(FilterContext context);

        Task<TResult> ProcessAsync<TResult>(IList<FilterContext> contexts);
    }
}