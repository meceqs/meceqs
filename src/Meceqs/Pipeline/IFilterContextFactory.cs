namespace Meceqs.Pipeline
{
    /// <summary>
    /// Creates a <see cref="FilterContext"/> for the given envelope.
    /// </summary>
    public interface IFilterContextFactory
    {
        /// <summary>
        /// Creates a <see cref="FilterContext"/> for the given <paramref name="envelope"/>.
        /// </summary>
        FilterContext CreateFilterContext(Envelope envelope);
    }
}