using Meceqs;
using Meceqs.Configuration;
using Meceqs.Serialization;
using Meceqs.Serialization.Json;
using Newtonsoft.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class JsonSerializationMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddJsonSerialization(this IMeceqsBuilder builder, JsonSerializerSettings settings = null)
        {
            Check.NotNull(builder, nameof(builder));

            var serializer = new JsonEnvelopeSerializer(settings);

            builder.Services.AddSingleton<IEnvelopeSerializer>(serializer);
            builder.Services.AddSingleton<IResultSerializer>(serializer);

            builder.Services.AddSingleton<IEnvelopeDeserializer, JsonEnvelopeDeserializer>();
            builder.Services.AddSingleton<IResultDeserializer, JsonEnvelopeDeserializer>();

            return builder;
        }
    }
}