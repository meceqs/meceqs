using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Configuration
{
    /// <summary>
    /// Used to configure the behavior of Meceqs.
    /// </summary>
    public interface IMeceqsBuilder
    {
        IServiceCollection Services { get; }
    }
}