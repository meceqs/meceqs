using System.Reflection;
using Meceqs;
using Meceqs.Configuration;
using Meceqs.Serialization;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SerializationMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddSerialization(this IMeceqsBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.TryAddSingleton<IEnvelopeTypeLoader, DefaultEnvelopeTypeLoader>();

            return builder;
        }

        public static IMeceqsBuilder AddSerializer<TEnvelopeSerializer>(this IMeceqsBuilder builder)
            where TEnvelopeSerializer : class, IEnvelopeSerializer
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.AddSingleton<IEnvelopeSerializer, TEnvelopeSerializer>();

            return builder;
        }

        public static IMeceqsBuilder AddDeserializer<TEnvelopeDeserializer>(this IMeceqsBuilder builder)
            where TEnvelopeDeserializer : class, IEnvelopeDeserializer
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.AddSingleton<IEnvelopeDeserializer, TEnvelopeDeserializer>();

            return builder;
        }

        public static IMeceqsBuilder AddDeserializationAssembly<TType>(this IMeceqsBuilder builder)
        {
            var assembly = typeof(TType).GetTypeInfo().Assembly;

            return builder.AddDeserializationAssembly(assembly);
        }

        public static IMeceqsBuilder AddDeserializationAssembly(this IMeceqsBuilder builder, params Assembly[] assemblies)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(assemblies, nameof(assemblies));

            builder.Services.Configure<EnvelopeTypeLoaderOptions>(options => options.TryAddContractAssembly(assemblies));

            return builder;
        }
    }
}