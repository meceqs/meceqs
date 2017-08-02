using System;
using Newtonsoft.Json.Serialization;
using NJsonSchema;
using NJsonSchema.Generation;

namespace Meceqs.AspNetCore.Swagger
{
    public class MeceqsToSwaggerGeneratorSettings : JsonSchemaGeneratorSettings
    {
        public MeceqsToSwaggerGeneratorSettings()
        {
            NullHandling = NullHandling.Swagger;
            DefaultPropertyNameHandling = PropertyNameHandling.CamelCase;
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Version { get; set; } = "1.0.0";

        internal JsonContract ResolveContract(Type parameterType)
        {
            return ActualContractResolver.ResolveContract(parameterType);
        }
    }
}