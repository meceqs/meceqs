using System.Threading.Tasks;

namespace Meceqs.Pipeline
{
    /// <summary>
    /// Represents a pointer to the "Invoke" method of the next middleware in a pipeline.
    /// </summary>
    public delegate Task MessageDelegate(MessageContext context);
}