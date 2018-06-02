﻿using Meceqs.AspNetCore.Configuration;
using Meceqs.AspNetCore.Receiving;
using Meceqs.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace Meceqs.AspNetCore.Swagger
{

    public class MeceqsDocumentFilter : IDocumentFilter
    {
        private readonly AspNetCoreReceiverOptions _options;
        private readonly IMessagePathConvention _messagePathConvention;
        private readonly MeceqsSwaggerOptions _meceqsOptions;

        public MeceqsDocumentFilter(
            IOptions<AspNetCoreReceiverOptions> options,
            IMessagePathConvention messagePathConvention,
            MeceqsSwaggerOptions meceqsOptions)
        {
            _options = options?.Value;
            _messagePathConvention = messagePathConvention;
            _meceqsOptions = meceqsOptions;
        }

        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var messageType in _options.MessageTypes)
            {
                AddMessageType(messageType, swaggerDoc, context.SchemaRegistry);
            }

            swaggerDoc.Definitions = context.SchemaRegistry.Definitions;
        }

        private void AddMessageType(MessageMetadata messageType, SwaggerDocument document, ISchemaRegistry schemaRegistry)
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

            // Body Parameters

            var envelopeType = typeof(Envelope<>).MakeGenericType(messageType.MessageType);
            var envelopeSchema = schemaRegistry.GetOrRegister(envelopeType);

            var parameter = new BodyParameter
            {
                Name = "envelope",
                Required = true,
                Schema = envelopeSchema
            };

            operation.Parameters.Add(parameter);

            // Response

            operation.Responses.Add("200", new Response
            {
                Description = "OK",
                Schema = messageType.ResultType != typeof(void) ? schemaRegistry.GetOrRegister(messageType.ResultType) : null
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
            path = AspNetCoreReceiverUtils.CombineRoutePrefixAndMessagePath(_options.RoutePrefix, path);

            document.Paths.Add(path, new PathItem
            {
                Post = operation
            });
        }
    }
}
