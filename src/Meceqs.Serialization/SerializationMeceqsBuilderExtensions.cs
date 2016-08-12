using System.Reflection;
using Meceqs;
using Meceqs.Configuration;
using Meceqs.Serialization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SerializationMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddSerialization(this IMeceqsBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.AddSingleton<IEnvelopeTypeLoader, DefaultEnvelopeTypeLoader>();

            return builder;
        }

        public static IMeceqsBuilder AddContractAssembly<TType>(this IMeceqsBuilder builder)
        {
            var assembly = typeof(TType).GetTypeInfo().Assembly;

            return builder.AddContractAssemblies(assembly);
        }

        public static IMeceqsBuilder AddContractAssemblies(this IMeceqsBuilder builder, params Assembly[] assemblies)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(assemblies, nameof(assemblies));

            builder.Services.Configure<MeceqsSerializationOptions>(options => options.ContractAssemblies.AddRange(assemblies));
            
            return builder;
        }
    }
}