using System.Reflection;
using Meceqs;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddContractAssembly<TType>(this IMeceqsBuilder builder)
        {
            var assembly = typeof(TType).GetTypeInfo().Assembly;

            return builder.AddContractAssembly(assembly);
        }

        public static IMeceqsBuilder AddContractAssembly(this IMeceqsBuilder builder, Assembly assembly)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(assembly, nameof(assembly));

            builder.Services.Configure<MeceqsOptions>(options => options.ContractAssemblies.Add(assembly));
            
            return builder;
        }
    }
}