using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Configuration
{
    public interface IMeceqsBuilder
    {
        IServiceCollection Services { get; }
    }
}