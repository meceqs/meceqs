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
        /// Processes the <paramref name="context"/> on the pipeline without expecting a result.
        /// </summary>
        Task ProcessAsync(FilterContext context);

        /// <summary>
        /// <para>Processes the <paramref name="context"/> on the pipeline and returns the given result.</para>
        /// <para>If the actual result object has a different type, this will throw a <see cref="System.InvalidCastException"/></para>
        /// </summary>
        Task<TResult> ProcessAsync<TResult>(FilterContext context);
    }
}