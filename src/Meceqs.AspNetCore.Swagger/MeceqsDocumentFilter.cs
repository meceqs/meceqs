using System;
using Meceqs.AspNetCore.Receiving;
using Meceqs.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using Meceqs.Serialization;

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

        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (string receiverName in _transportOptions.Receivers)
            {
                AspNetCoreReceiverOptions receiverOptions = _receiverOptions.Get(receiverName);

                foreach (var messageType in receiverOptions.MessageTypes.OrderBy(x => x.MessageType.FullName))
                {
                    AddMessageType(messageType, swaggerDoc, context.SchemaRegistry, receiverOptions);
                }
            }

            swaggerDoc.Definitions = context.SchemaRegistry.Definitions;
        }

        private void AddMessageType(MessageMetadata messageType, SwaggerDocument document, ISchemaRegistry schemaRegistry, AspNetCoreReceiverOptions receiverOptions)
        {
            if (document.Paths == null)
            {
                document.Paths = new Dictionary<string, PathItem>();
            }

            var operation = new Operation
            {
                Parameters = new List<IParameter>(),
                Responses = new Dictionary<string, Response>()
            };

            // Content Types

            operation.Consumes = GetOrderedContentTypes(messageType.MessageType);

            if (messageType.ResponseType != typeof(void))
            {
                operation.Produces = GetOrderedContentTypes(messageType.ResponseType);
            }

            // Parameters

            operation.Parameters.Add(new BodyParameter
            {
                Name = "message",
                Required = true,
                Schema = schemaRegistry.GetOrRegister(messageType.MessageType),
            });

            operation.Parameters.Add(new NonBodyParameter
            {
                Name = TransportHeaderNames.MessageId,
                In = "header",
                Type = "string",
                Format = "uuid"
            });

            operation.Parameters.Add(new NonBodyParameter
            {
                Name = TransportHeaderNames.CorrelationId,
                In = "header",
                Type = "string",
                Format = "uuid"
            });

            // Response

            operation.Responses.Add("200", new Response
            {
                Description = "OK",
                Schema = messageType.ResponseType != typeof(void) ? schemaRegistry.GetOrRegister(messageType.ResponseType) : null
            });

            if (!string.IsNullOrEmpty(_meceqsOptions.SecurityDefinition))
            {
                operation.Responses.Add("401", new Response { Description = "Unauthorized" });
                operation.Responses.Add("403", new Response { Description = "Forbidden" });

                operation.Security = new List<IDictionary<string, IEnumerable<string>>>
                {
                    new Dictionary<string, IEnumerable<string>>
                    {
                        { _meceqsOptions.SecurityDefinition, Enumerable.Empty<string>() }
                    }
                };
            }

            string path = _messagePathConvention.GetPathForMessage(messageType.MessageType);
            path = AspNetCoreReceiverUtils.CombineRoutePrefixAndMessagePath(receiverOptions.RoutePrefix, path);

            document.Paths.Add(path, new PathItem
            {
                Post = operation
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
