using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meceqs.AspNetCore.Configuration;
using Meceqs.AspNetCore.Receiving;
using Meceqs.Transport;
using Microsoft.AspNetCore.Hosting;
using NJsonSchema;
using NSwag;
using NSwag.SwaggerGeneration;

namespace Meceqs.AspNetCore.Swagger
{
    public class MeceqsToSwaggerGenerator
    {
        private readonly MeceqsToSwaggerGeneratorSettings _settings;
        private readonly SwaggerJsonSchemaGenerator _schemaGenerator;

        private readonly AspNetCoreReceiverOptions _options;
        private readonly IMessagePathConvention _messagePathConvention;
        private readonly IHostingEnvironment _hostingEnvironment;

        public MeceqsToSwaggerGenerator(
            MeceqsToSwaggerGeneratorSettings settings,
            AspNetCoreReceiverOptions options,
            IMessagePathConvention messagePathConvention,
            IHostingEnvironment hostingEnvironment)
        {
            _settings = settings;
            _schemaGenerator = new SwaggerJsonSchemaGenerator(_settings);

            _options = options;
            _messagePathConvention = messagePathConvention;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<SwaggerDocument> CreateSwaggerDocument()
        {
            SwaggerDocument document = new SwaggerDocument();
            document.Consumes = new List<string> { "application/json" };
            document.Produces = new List<string> { "application/json" };
            document.Info = new SwaggerInfo
            {
                Title = _settings.Title ?? _hostingEnvironment.ApplicationName,
                Description = _settings.Description,
                Version = _settings.Version
            };

            var schemaResolver = new SwaggerSchemaResolver(document, _settings);
            var swaggerGenerator = new SwaggerGenerator(_schemaGenerator, _settings, schemaResolver);

            foreach (var messageType in _options.MessageTypes)
            {
                await AddMessageType(messageType, document, schemaResolver, swaggerGenerator);
            }

            AppendRequiredSchemasToDefinitions(document, schemaResolver);
            document.GenerateOperationIds();

            return document;
        }

        private async Task AddMessageType(
            MessageMetadata messageType,
            SwaggerDocument document,
            SwaggerSchemaResolver schemaResolver,
            SwaggerGenerator swaggerGenerator)
        {
            string path = _messagePathConvention.GetPathForMessage(messageType.MessageType);
            var operationDescription = new SwaggerOperationDescription
            {
                Path = "/" + path,
                Method = SwaggerOperationMethod.Post,
                Operation = new SwaggerOperation
                {
                    OperationId = path
                }
            };

            // Body Parameters

            var parameter = new SwaggerParameter
            {
                Name = "message",
                Kind = SwaggerParameterKind.Body,
                IsRequired = true,
                IsNullableRaw = false,
                Schema = await _schemaGenerator.GenerateWithReference<JsonSchema4>(
                    messageType.MessageType,
                    Enumerable.Empty<Attribute>(),
                    schemaResolver)
            };

            operationDescription.Operation.Parameters.Add(parameter);

            // Response

            SwaggerResponse response;
            if (messageType.ResultType != typeof(void))
            {
                response = new SwaggerResponse
                {
                    Schema = await _schemaGenerator.GenerateWithReference<JsonSchema4>(
                        messageType.ResultType,
                        Enumerable.Empty<Attribute>(),
                        schemaResolver)
                };
            }
            else
            {
                response = new SwaggerResponse();
            }

            operationDescription.Operation.Responses.Add("200", response);

            document.Paths[operationDescription.Path] = new SwaggerPathItem();
            document.Paths[operationDescription.Path][operationDescription.Method] = operationDescription.Operation;
        }

        private void AppendRequiredSchemasToDefinitions(SwaggerDocument document, SwaggerSchemaResolver schemaResolver)
        {
            foreach (var schema in schemaResolver.Schemas)
            {
                if (!document.Definitions.Values.Contains(schema))
                {
                    var typeName = _settings.TypeNameGenerator.Generate(schema, string.Empty, null);

                    if (!document.Definitions.ContainsKey(typeName))
                        document.Definitions[typeName] = schema;
                    else
                        document.Definitions["ref_" + Guid.NewGuid().ToString().Replace("-", "_")] = schema;
                }
            }
        }
    }
}