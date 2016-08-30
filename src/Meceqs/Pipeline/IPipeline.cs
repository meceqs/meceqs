using System.Threading.Tasks;

namespace Meceqs.Pipeline
{
    /// <summary>
    /// Represents an in-memory channel consisting of multiple "filters".
    /// </summary>
    public interface IPipeline
    {
        Task ProcessAsync(FilterContext context);

        Task<TResult> ProcessAsync<TResult>(FilterContext context);
    }
}