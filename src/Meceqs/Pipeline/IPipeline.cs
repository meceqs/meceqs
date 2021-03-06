using System.Threading.Tasks;

namespace Meceqs.Pipeline
{
    /// <summary>
    /// A pipeline represents a list of independent middleware components.
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
        Task InvokeAsync(MessageContext context);
    }
}
