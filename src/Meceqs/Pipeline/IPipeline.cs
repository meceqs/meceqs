using System.Threading.Tasks;

namespace Meceqs.Pipeline
{
    /// <summary>
    /// A pipeline represents a list of independant filters.
    /// </summary>
    public interface IPipeline
    {
        /// <summary>
        /// The name of the pipeline.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Processes the <paramref name="context"/> on the pipeline.
        /// </summary>
        Task InvokeAsync(FilterContext context);
    }
}