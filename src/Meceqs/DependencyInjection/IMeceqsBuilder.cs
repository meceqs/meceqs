using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Used to configure the behavior of Meceqs.
    /// </summary>
    public interface IMeceqsBuilder
    {
        /// <summary>
        /// Gets the application service collection.
        /// </summary>
        IServiceCollection Services { get; }

        /// <summary>
        /// Gets the configuration for Meceqs.
        /// </summary>
        IConfiguration Configuration { get; }
    }
}