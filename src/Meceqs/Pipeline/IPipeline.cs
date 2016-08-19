using System.Threading.Tasks;

namespace Meceqs.Pipeline
{
    public interface IPipeline
    {
        Task ProcessAsync(FilterContext context);

        Task<TResult> ProcessAsync<TResult>(FilterContext context);
    }
}