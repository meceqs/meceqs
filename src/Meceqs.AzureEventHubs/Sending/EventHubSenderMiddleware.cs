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
        // TODO EventHubClient lifecycle - should it be transient?

        private readonly ILogger _logger;
        private readonly IEventHubClient _eventHubClient;

        public EventHubSenderMiddleware(
            MiddlewareDelegate next,
            IOptions<EventHubSenderOptions> options,
            ILoggerFactory loggerFactory,
            IEventHubClientFactory eventHubClientFactory)
        {
            // "next" is not stored because this is a terminating middleware

            Guard.NotNull(options?.Value, nameof(options.Value));
            Guard.NotNull(loggerFactory, nameof(loggerFactory));
            Guard.NotNull(eventHubClientFactory, nameof(eventHubClientFactory));

            _logger = loggerFactory.CreateLogger<EventHubSenderMiddleware>();

            _eventHubClient = eventHubClientFactory.CreateEventHubClient(options.Value.EventHubConnectionString);
        }

        public async Task Invoke(MessageContext context, IEventDataConverter eventDataConverter)
        {
            Guard.NotNull(context, nameof(context));
            Guard.NotNull(eventDataConverter, nameof(eventDataConverter));

            // TODO @cweiss which LogLevel?
            _logger.LogInformation("Sending message {MessageType}/{MessageId}", context.Envelope.MessageType, context.Envelope.MessageId);

            var eventData = eventDataConverter.ConvertToEventData(context.Envelope);

            // TODO @cweiss move magic string somewhere.
            string partitionKey = context.Items.Get<object>("PartitionKey")?.ToString();

            await _eventHubClient.SendAsync(eventData, partitionKey);
        }

        public void Dispose()
        {
            // Note: http://vunvulearadu.blogspot.co.at/2016/01/why-connection-is-not-closing-when-i.html
            _eventHubClient?.Close();
        }
    }
}