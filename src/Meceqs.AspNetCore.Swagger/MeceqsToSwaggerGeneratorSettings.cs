using NJsonSchema;
using NJsonSchema.Generation;

namespace Meceqs.AspNetCore.Swagger
{
    public class MeceqsToSwaggerGeneratorSettings : JsonSchemaGeneratorSettings
    {
        public MeceqsToSwaggerGeneratorSettings()
        {
            DefaultPropertyNameHandling = PropertyNameHandling.CamelCase;
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Version { get; set; } = "1.0.0";
    }
}