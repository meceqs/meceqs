using System;
using System.Threading.Tasks;
using Meceqs.AzureEventHubs.Internal;
using Meceqs.Pipeline;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Meceqs.AzureEventHubs.Sending
{
    public class EventHubSenderMiddleware : IDisposable
    {
        // TODO EventHubClient lifecycle - should it be static?

        private readonly IOptionsMonitor<EventHubSenderOptions> _optionsMonitor;
        private readonly ILogger _logger;
        private readonly IEventHubClientFactory _eventHubClientFactory;

        private readonly object _lock = new object();
        private IEventHubClient _eventHubClient;

        public EventHubSenderMiddleware(
            MiddlewareDelegate next,
            IOptionsMonitor<EventHubSenderOptions> optionsMonitor,
            ILoggerFactory loggerFactory,
            IEventHubClientFactory eventHubClientFactory)
        {
            // "next" is not stored because this is a terminating middleware

            Guard.NotNull(optionsMonitor, nameof(optionsMonitor));
            Guard.NotNull(loggerFactory, nameof(loggerFactory));
            Guard.NotNull(eventHubClientFactory, nameof(eventHubClientFactory));

            _optionsMonitor = optionsMonitor;
            _logger = loggerFactory.CreateLogger<EventHubSenderMiddleware>();
            _eventHubClientFactory = eventHubClientFactory;
        }

        public async Task Invoke(MessageContext context, IEventDataConverter eventDataConverter)
        {
            Guard.NotNull(context, nameof(context));
            Guard.NotNull(eventDataConverter, nameof(eventDataConverter));

            _logger.LogDebug("Sending message {MessageType}/{MessageId}", context.Envelope.MessageType, context.Envelope.MessageId);

            var eventData = eventDataConverter.ConvertToEventData(context.Envelope);

            // TODO @cweiss move magic string somewhere.
            string partitionKey = context.Items.Get<object>("PartitionKey")?.ToString();

            var eventHubClient = GetOrCreateClient(context.PipelineName);

            await eventHubClient.SendAsync(eventData, partitionKey);
        }

        public void Dispose()
        {
            // Note: http://vunvulearadu.blogspot.co.at/2016/01/why-connection-is-not-closing-when-i.html
            _eventHubClient?.Close();
        }

        private IEventHubClient GetOrCreateClient(string pipelineName)
        {
            if (_eventHubClient != null)
                return _eventHubClient;

            lock (_lock)
            {
                if (_eventHubClient != null)
                    return _eventHubClient;

                var options = _optionsMonitor.Get(pipelineName);
                _eventHubClient = _eventHubClientFactory.CreateEventHubClient(options.EventHubConnectionString);
                return _eventHubClient;
            }
        }
    }
}