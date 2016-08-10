using System.Collections.Generic;
using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Meceqs.Channels
{
    public interface IChannel
    {
        Task SendAsync(IList<FilterContext> contexts);

        Task<TResult> SendAsync<TResult>(IList<FilterContext> filterContexts);
    }
}