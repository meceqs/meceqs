using System.Threading.Tasks;

namespace Meceqs.Pipeline
{
    /// <summary>
    /// Represents a pointer to the "Invoke" method of the next filter in a pipeline.
    /// </summary>
    public delegate Task FilterDelegate(FilterContext context);
}