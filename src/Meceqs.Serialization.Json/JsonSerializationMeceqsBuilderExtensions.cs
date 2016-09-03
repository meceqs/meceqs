using Meceqs;
using Meceqs.Configuration;
using Meceqs.Serialization.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class JsonSerializationMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddJsonSerialization(this IMeceqsBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.AddSerializer<JsonEnvelopeSerializer>();
            builder.AddDeserializer<JsonEnvelopeDeserializer>();

            return builder;
        }
    }
}