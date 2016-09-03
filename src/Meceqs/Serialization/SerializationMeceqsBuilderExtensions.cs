using System.Reflection;
using Meceqs;
using Meceqs.Configuration;
using Meceqs.Transport;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TransportMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddSerializer<TEnvelopeSerializer>(this IMeceqsBuilder builder)
            where TEnvelopeSerializer : class, IEnvelopeSerializer
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.TryAddSingleton<IEnvelopeTypeLoader, DefaultEnvelopeTypeLoader>();
            builder.Services.TryAddSingleton<IEnvelopeSerializer, TEnvelopeSerializer>();

            return builder;
        }

        public static IMeceqsBuilder AddDeserializer<TEnvelopeDeserializer>(this IMeceqsBuilder builder)
            where TEnvelopeDeserializer : class, IEnvelopeDeserializer
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.TryAddSingleton<IEnvelopeTypeLoader, DefaultEnvelopeTypeLoader>();
            builder.Services.TryAddSingleton<IEnvelopeDeserializer, TEnvelopeDeserializer>();

            return builder;
        }

        public static IMeceqsBuilder AddDeserializationAssembly<TType>(this IMeceqsBuilder builder)
        {
            var assembly = typeof(TType).GetTypeInfo().Assembly;

            return builder.AddDeserializationAssemblies(assembly);
        }

        public static IMeceqsBuilder AddDeserializationAssemblies(this IMeceqsBuilder builder, params Assembly[] assemblies)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(assemblies, nameof(assemblies));

            builder.Services.Configure<MeceqsTransportOptions>(options => options.ContractAssemblies.AddRange(assemblies));
            
            return builder;
        }
    }
}