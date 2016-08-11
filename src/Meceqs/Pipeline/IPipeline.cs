using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meceqs.Pipeline
{
    public interface IPipeline
    {
        Task<TResult> ProcessAsync<TResult>(IList<FilterContext> filterContexts);
    }
}