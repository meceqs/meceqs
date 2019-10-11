using System;
using System.Collections.Generic;
using System.Linq;
using Meceqs.AspNetCore.Receiving;
using Meceqs.Serialization;
using Meceqs.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Meceqs.AspNetCore.Swagger
{
    public class MeceqsDocumentFilter : IDocumentFilter
    {
        private const string JsonContentType = "application/json";

        private readonly ReceiveEndpointOptions _transportOptions;
        private readonly IOptionsMonitor<AspNetCoreReceiverOptions> _receiverOptions;
        private readonly IMessagePathConvention _messagePathConvention;
        private readonly ISerializationProvider _serializationProvider;
        private readonly MeceqsSwaggerOptions _meceqsOptions;

        public MeceqsDocumentFilter(
            IOptions<ReceiveEndpointOptions> transportOptions,
            IOptionsMonitor<AspNetCoreReceiverOptions> receiverOptions,
            IMessagePathConvention messagePathConvention,
            ISerializationProvider serializationProvider,
            MeceqsSwaggerOptions meceqsOptions)
        {
            _transportOptions = transportOptions?.Value;
            _receiverOptions = receiverOptions;
            _messagePathConvention = messagePathConvention;
            _serializationProvider = serializationProvider;
            _meceqsOptions = meceqsOptions;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (string receiverName in _transportOptions.Receivers)
            {
                AspNetCoreReceiverOptions receiverOptions = _receiverOptions.Get(receiverName);

                foreach (var messageType in receiverOptions.MessageTypes.OrderBy(x => x.MessageType.FullName))
                {
                    AddMessageType(messageType, swaggerDoc, context.SchemaGenerator, context.SchemaRepository, receiverOptions);
                }
            }

            swaggerDoc.Components.Schemas = context.SchemaRepository.Schemas;
        }

        private void AddMessageType(
            MessageMetadata messageType, 
            OpenApiDocument document, 
            ISchemaGenerator schemaGenerator, 
            SchemaRepository schemaRepository, 
            AspNetCoreReceiverOptions receiverOptions)
        {
            if (document.Paths == null)
            {
                document.Paths = new OpenApiPaths();
            }

            var operation = new OpenApiOperation
            {
                Parameters = new List<OpenApiParameter>(),
                Responses = new OpenApiResponses()
            };

            // Request body

            operation.RequestBody = new OpenApiRequestBody
            {
                Required = true,
                Content = GetOrderedContentTypes(messageType.MessageType).ToDictionary(x => x, _ => new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Reference = schemaGenerator.GenerateSchema(messageType.MessageType, schemaRepository).Reference
                    }
                })
            };
            
            // Other parameters

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = TransportHeaderNames.MessageId,
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Format = "uuid"
                }
            });

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = TransportHeaderNames.CorrelationId,
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Format = "uuid"
                }
            });

            // Response

            operation.Responses.Add("200", new OpenApiResponse
            {
                Description = "OK",
                Content = messageType.ResponseType == typeof(void) 
                    ? null 
                    : GetOrderedContentTypes(messageType.ResponseType).ToDictionary(x => x, _ => new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Reference = schemaGenerator.GenerateSchema(messageType.ResponseType, schemaRepository).Reference
                        }
                    })
            });

            if (!string.IsNullOrEmpty(_meceqsOptions.SecurityDefinition))
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = _meceqsOptions.SecurityDefinition
                                }
                            },
                            new List<string>()
                        }
                    }
                };
            }

            string path = _messagePathConvention.GetPathForMessage(messageType.MessageType);
            path = AspNetCoreReceiverUtils.CombineRoutePrefixAndMessagePath(receiverOptions.RoutePrefix, path);

            document.Paths.Add(path, new OpenApiPathItem
            {
                Operations =
                {
                    { OperationType.Post, operation }
                }
            });
        }

        private List<string> GetOrderedContentTypes(Type objectType)
        {
            var contentTypes = _serializationProvider.GetSupportedContentTypes(objectType).ToList();

            // If JSON is supported, we place it at the beginning so that the Swagger UI automatically selects it.
            // This makes using the UI much easier if you have other non-textual serializers.
            if (contentTypes.Count > 1 && contentTypes.Remove(JsonContentType))
            {
                contentTypes.Insert(0, JsonContentType);
            }

            return contentTypes;
        }
    }
}
