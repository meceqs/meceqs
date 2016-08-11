using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meceqs.Pipeline
{
    public interface IPipeline
    {
        Task SendAsync(IList<FilterContext> contexts);

        Task<TResult> SendAsync<TResult>(IList<FilterContext> filterContexts);
    }
}