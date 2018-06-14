namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Used to configure the behavior of Meceqs.
    /// </summary>
    public interface IMeceqsBuilder
    {
        IServiceCollection Services { get; }
    }
}