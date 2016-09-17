using System.Threading.Tasks;

namespace Meceqs.Pipeline
{
    /// <summary>
    /// Represents an in-memory channel consisting of multiple "filters".
    /// </summary>
    public interface IPipeline
    {
        string Name { get; }

        Task ProcessAsync(FilterContext context);

        Task<TResult> ProcessAsync<TResult>(FilterContext context);
    }
}