using System.Reflection;
using Meceqs;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddContractAssembly<TType>(this IMeceqsBuilder builder)
        {
            var assembly = typeof(TType).GetTypeInfo().Assembly;

            return builder.AddContractAssemblies(assembly);
        }

        public static IMeceqsBuilder AddContractAssemblies(this IMeceqsBuilder builder, params Assembly[] assemblies)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(assemblies, nameof(assemblies));

            builder.Services.Configure<MeceqsOptions>(options => options.ContractAssemblies.AddRange(assemblies));
            
            return builder;
        }
    }
}