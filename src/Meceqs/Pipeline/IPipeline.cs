using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meceqs.Pipeline
{
    public interface IPipeline
    {
        Task ProcessAsync(IList<FilterContext> contexts);

        Task<TResult> ProcessAsync<TResult>(IList<FilterContext> filterContexts);
    }
}