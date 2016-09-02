using System;
using System.Threading.Tasks;
using Meceqs.Pipeline;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureServiceBus.Filters
{
    public class EventHubPublisherFilter : IDisposable
    {
        // TODO EventHubClient lifecycle - should it be transient?

        private readonly ILogger _logger;
        private readonly EventHubClient _eventHubClient;

        public EventHubPublisherFilter(
            FilterDelegate next,
            IOptions<EventHubPublisherOptions> options,
            ILoggerFactory loggerFactory,
            IEventHubClientFactory eventHubClientFactory)
        {
            // "next" is not stored because this is a terminating filter

            Check.NotNull(options?.Value, nameof(options.Value));
            Check.NotNull(loggerFactory, nameof(loggerFactory));
            Check.NotNull(eventHubClientFactory, nameof(eventHubClientFactory));

            _logger = loggerFactory.CreateLogger<EventHubPublisherFilter>();

            var connection = new EventHubConnection(options.Value.EventHubConnectionString);
            _eventHubClient = eventHubClientFactory.CreateEventHubClient(connection);
        }

        public async Task Invoke(FilterContext context, IEventDataConverter eventDataConverter)
        {
            Check.NotNull(context, nameof(context));
            Check.NotNull(eventDataConverter, nameof(eventDataConverter));

            _logger.LogInformation("Publishing message {MessageName}/{MessageId}", context.Envelope.MessageName, context.Envelope.MessageId);

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