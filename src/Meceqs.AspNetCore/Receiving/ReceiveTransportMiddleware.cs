using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meceqs.Transport;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Meceqs.AspNetCore.Receiving
{
    /// <summary>
    /// This is an ASP.NET Core middleware that "receives" incoming HTTP requests.
    /// </summary>
    public class ReceiveTransportMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ReceiveTransportOptions _transportOptions;
        private readonly IOptionsMonitor<AspNetCoreReceiverOptions> _receiverOptionsMonitor;
        private readonly ILogger _logger;

        private readonly Dictionary<string, Tuple<string, MessageMetadata>> _pathLookup;

        public ReceiveTransportMiddleware(
            RequestDelegate next,
            IOptions<ReceiveTransportOptions> transportOptions,
            IOptionsMonitor<AspNetCoreReceiverOptions> receiverOptionsMonitor,
            IMessagePathConvention messagePathConvention,
            ILoggerFactory loggerFactory)
        {
            Guard.NotNull(next, nameof(next));
            Guard.NotNull(transportOptions?.Value, nameof(transportOptions));
            Guard.NotNull(receiverOptionsMonitor, nameof(receiverOptionsMonitor));
            Guard.NotNull(messagePathConvention, nameof(messagePathConvention));
            Guard.NotNull(loggerFactory, nameof(loggerFactory));

            _next = next;
            _transportOptions = transportOptions.Value;
            _receiverOptionsMonitor = receiverOptionsMonitor;
            _logger = loggerFactory.CreateLogger<ReceiveTransportMiddleware>();

            _pathLookup = BuildPathLookup(messagePathConvention);
        }

        public Task Invoke(HttpContext httpContext, IAspNetCoreReceiver aspNetCoreReceiver)
        {
            Guard.NotNull(httpContext, nameof(httpContext));
            Guard.NotNull(aspNetCoreReceiver, nameof(aspNetCoreReceiver));

            if (_pathLookup.TryGetValue(httpContext.Request.Path, out var tuple))
            {
                string receiverName = tuple.Item1;
                MessageMetadata messageMetadata = tuple.Item2;

                _logger.LogDebug("Path matched for message type {Receiver}/{MessageType}", receiverName, messageMetadata.MessageType);

                return aspNetCoreReceiver.ReceiveAsync(httpContext, receiverName, messageMetadata);
            }
            else
            {
                _logger.LogDebug("No matching path found for '{Path}'", httpContext.Request.Path);

                // someone else might handle it, or it will result in a 404.
                return _next(httpContext);
            }
        }

        private Dictionary<string, Tuple<string, MessageMetadata>> BuildPathLookup(IMessagePathConvention messagePathConvention)
        {
            var lookup = new Dictionary<string, Tuple<string, MessageMetadata>>(StringComparer.OrdinalIgnoreCase);

            if (_transportOptions.Receivers.Count == 0)
            {
                throw new MeceqsException("No receivers have been configured for the ASP.NET Core transport.");
            }

            foreach (string receiverName in _transportOptions.Receivers)
            {
                var receiverOptions = _receiverOptionsMonitor.Get(receiverName);

                if (receiverOptions.MessageTypes.Count == 0)
                {
                    throw new MeceqsException($"There are no configured handlers in {nameof(AspNetCoreReceiverOptions)}.{nameof(receiverOptions.MessageTypes)}");
                }

                foreach (var metadata in receiverOptions.MessageTypes)
                {
                    var path = messagePathConvention.GetPathForMessage(metadata.MessageType);
                    path = AspNetCoreReceiverUtils.CombineRoutePrefixAndMessagePath(receiverOptions.RoutePrefix, path);

                    lookup.Add(path, Tuple.Create(receiverName, metadata));
                }
            }

            // TODO remove?
            _logger.LogDebug("PathLookup: {PathLookup}", string.Join(" ", lookup.Select(x => $"{x.Key}=>{x.Value.Item1}:{x.Value.Item2.MessageType.Name}")));

            return lookup;
        }
    }
}