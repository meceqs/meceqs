namespace Meceqs.Pipeline
{
    /// <summary>
    /// Provides a way for frameworks to automatically set additional properties on every <see cref="FilterContext"/>
    /// before the pipeline is executed.
    /// </summary>
    /// <remarks>
    /// As an example, in ASP.NET Core an implementation could get the per-request service provider
    /// from "IHttpContextAccessor" and pass it to <see cref="FilterContext.RequestServices"/>.
    /// </remarks>
    public interface IFilterContextEnricher
    {
        void EnrichFilterContext(FilterContext context);
    }
}