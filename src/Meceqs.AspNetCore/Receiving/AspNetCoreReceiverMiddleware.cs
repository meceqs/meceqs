using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meceqs.AspNetCore.Configuration;
using Meceqs.Transport;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Meceqs.AspNetCore.Receiving
{
    public class AspNetCoreReceiverMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AspNetCoreReceiverOptions _options;
        private readonly ILogger _logger;

        private readonly Dictionary<string, MessageMetadata> _pathLookup;

        public AspNetCoreReceiverMiddleware(
            RequestDelegate next,
            IOptions<AspNetCoreReceiverOptions> options,
            IMessagePathConvention messagePathConvention,
            ILoggerFactory loggerFactory)
        {
            Guard.NotNull(next, nameof(next));
            Guard.NotNull(options?.Value, nameof(options));
            Guard.NotNull(messagePathConvention, nameof(messagePathConvention));
            Guard.NotNull(loggerFactory, nameof(loggerFactory));

            _next = next;
            _options = options.Value;
            _logger = loggerFactory.CreateLogger<AspNetCoreReceiverMiddleware>();

            _pathLookup = BuildPathLookup(messagePathConvention);
        }

        public Task Invoke(HttpContext httpContext, IAspNetCoreReceiver aspNetCoreReceiver)
        {
            Guard.NotNull(httpContext, nameof(httpContext));
            Guard.NotNull(aspNetCoreReceiver, nameof(aspNetCoreReceiver));

            MessageMetadata messageMetadata;
            if (_pathLookup.TryGetValue(httpContext.Request.Path, out messageMetadata))
            {
                _logger.LogDebug("Path matched for message type {MessageType}", messageMetadata.MessageType);

                return aspNetCoreReceiver.ReceiveAsync(httpContext, messageMetadata);
            }
            else
            {
                _logger.LogDebug("No matching path found for '{Path}'", httpContext.Request.Path);

                // someone else might handle it, or it will result in a 404.
                return _next(httpContext);
            }
        }

        private Dictionary<string, MessageMetadata> BuildPathLookup(IMessagePathConvention messagePathConvention)
        {
            if (_options.MessageTypes.Count == 0)
            {
                throw new MeceqsException($"There are no configured handlers in {nameof(AspNetCoreReceiverOptions)}.{nameof(_options.MessageTypes)}");
            }

            var lookup = new Dictionary<string, MessageMetadata>(StringComparer.OrdinalIgnoreCase);

            foreach (var metadata in _options.MessageTypes)
            {
                var path = messagePathConvention.GetPathForMessage(metadata.MessageType);
                path = AspNetCoreReceiverUtils.CombineRoutePrefixAndMessagePath(_options.RoutePrefix, path);

                lookup.Add(path, metadata);
            }

            // TODO remove?
            _logger.LogDebug("PathLookup: {PathLookup}", string.Join(" ", lookup.Select(x => $"{x.Key}=>{x.Value.MessageType.Name}")));

            return lookup;
        }
    }
}