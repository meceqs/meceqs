namespace Meceqs.Pipeline
{
    public interface IFilterContextFactory
    {
        FilterContext CreateFilterContext(Envelope envelope);
    }
}