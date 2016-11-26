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
            MessageDelegate next,
            IOptions<EventHubSenderOptions> options,
            ILoggerFactory loggerFactory,
            IEventHubClientFactory eventHubClientFactory)
        {
            // "next" is not stored because this is a terminating middleware

            Check.NotNull(options?.Value, nameof(options.Value));
            Check.NotNull(loggerFactory, nameof(loggerFactory));
            Check.NotNull(eventHubClientFactory, nameof(eventHubClientFactory));

            _logger = loggerFactory.CreateLogger<EventHubSenderMiddleware>();

            var connection = new EventHubConnection(options.Value.EventHubConnectionString);
            _eventHubClient = eventHubClientFactory.CreateEventHubClient(connection);
        }

        public async Task Invoke(MessageContext context, IEventDataConverter eventDataConverter)
        {
            Check.NotNull(context, nameof(context));
            Check.NotNull(eventDataConverter, nameof(eventDataConverter));

            // TODO @cweiss which LogLevel?
            _logger.LogInformation("Sending message {MessageType}/{MessageId}", context.Envelope.MessageType, context.Envelope.MessageId);

            var eventData = eventDataConverter.ConvertToEventData(context.Envelope);

            // TODO @cweiss move magic string somewhere.
            eventData.PartitionKey = context.Items.Get<string>("PartitionKey");

            await _eventHubClient.SendAsync(eventData);
        }

        public void Dispose()
        {
            // Note: http://vunvulearadu.blogspot.co.at/2016/01/why-connection-is-not-closing-when-i.html
            _eventHubClient?.Close();
        }
    }
}