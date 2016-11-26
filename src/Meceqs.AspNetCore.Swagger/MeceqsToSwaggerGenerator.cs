using System;
using System.Collections.Generic;
using System.Linq;
using Meceqs.AspNetCore.Configuration;
using Meceqs.AspNetCore.Receiving;
using Meceqs.Transport;
using Microsoft.AspNetCore.Hosting;
using NJsonSchema;
using NSwag;
using NSwag.CodeGeneration.SwaggerGenerators;

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

        public SwaggerDocument CreateSwaggerDocument()
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

            var schemaResolver = new SchemaResolver();
            var schemaDefinitionAppender = new SwaggerDocumentSchemaDefinitionAppender(document, _settings.TypeNameGenerator);
            var swaggerGenerator = new SwaggerGenerator(_schemaGenerator, _settings, schemaResolver, schemaDefinitionAppender);

            foreach (var messageType in _options.MessageTypes)
            {
                AddMessageType(document, messageType, swaggerGenerator);
            }

            AppendRequiredSchemasToDefinitions(document, schemaResolver);
            document.GenerateOperationIds();

            return document;
        }

        private void AddMessageType(SwaggerDocument document, MessageMetadata messageType, SwaggerGenerator swaggerGenerator)
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

            var envelopeType = typeof(Envelope<>).MakeGenericType(messageType.MessageType);

            var typeDescription = JsonObjectTypeDescription.FromType(
                envelopeType,
                Enumerable.Empty<Attribute>(),
                _settings.DefaultEnumHandling);

            var parameter = new SwaggerParameter
            {
                Name = "envelope",
                Kind = SwaggerParameterKind.Body,
                IsRequired = true,
                IsNullableRaw = false,
                Schema = swaggerGenerator.GenerateAndAppendSchemaFromType(envelopeType, false, Enumerable.Empty<Attribute>())
            };

            operationDescription.Operation.Parameters.Add(parameter);

            // Response

            SwaggerResponse response;
            if (messageType.ResultType != null)
            {
                response = new SwaggerResponse
                {
                    Schema = swaggerGenerator.GenerateAndAppendSchemaFromType(messageType.ResultType, false, Enumerable.Empty<Attribute>())
                };
            }
            else
            {
                response = new SwaggerResponse();
            }

            operationDescription.Operation.Responses.Add("200", response);

            document.Paths[operationDescription.Path] = new SwaggerOperations();
            document.Paths[operationDescription.Path][operationDescription.Method] = operationDescription.Operation;
        }

        private void AppendRequiredSchemasToDefinitions(SwaggerDocument document, ISchemaResolver schemaResolver)
        {
            foreach (var schema in schemaResolver.Schemas)
            {
                if (!document.Definitions.Values.Contains(schema))
                {
                    var typeName = schema.GetTypeName(_settings.TypeNameGenerator, string.Empty);

                    if (!document.Definitions.ContainsKey(typeName))
                        document.Definitions[typeName] = schema;
                    else
                        document.Definitions["ref_" + Guid.NewGuid().ToString().Replace("-", "_")] = schema;
                }
            }
        }
    }
}