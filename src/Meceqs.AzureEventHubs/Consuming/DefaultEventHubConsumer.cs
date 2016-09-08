using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.AzureEventHubs.Internal;
using Meceqs.Configuration;
using Meceqs.Consuming;
using Meceqs.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureEventHubs.Consuming
{
    public class DefaultEventHubConsumer : IEventHubConsumer
    {
        private readonly EventHubConsumerOptions _options;
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DefaultEventHubConsumer(
            IOptions<EventHubConsumerOptions> options,
            ILoggerFactory loggerFactory,
            IServiceScopeFactory serviceScopeFactory)
        {
            Check.NotNull(options?.Value, nameof(options));
            Check.NotNull(loggerFactory, nameof(loggerFactory));
            Check.NotNull(serviceScopeFactory, nameof(serviceScopeFactory));

            _options = options.Value;
            _logger = loggerFactory.CreateLogger<DefaultEventHubConsumer>();
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task ConsumeAsync(EventData eventData, CancellationToken cancellation)
        {
            Check.NotNull(eventData, nameof(eventData));

            // Make sure each log message contains data about the currently processed message.
            using (_logger.EventDataScope(eventData))
            {
                _logger.ConsumeStarting(eventData);

                long startTimestamp = DateTime.UtcNow.Ticks;
                bool success = false;

                try
                {
                    await CreateScopeAndInvokeMessageConsumer(eventData, cancellation);
                    success = true;
                }
                catch (Exception ex)
                {
                    success = false;
                    _logger.ConsumeFailed(eventData, ex);
                    throw;
                }
                finally
                {
                    _logger.ConsumeFinished(success, startTimestamp, DateTime.UtcNow.Ticks);
                }
            }
        }

        private async Task CreateScopeAndInvokeMessageConsumer(EventData eventData, CancellationToken cancellation)
        {
            // Handling a message from an underlying transport is similar to handling a HTTP-request.
            // We must make sure processing of one message doesn't have an effect on other messages.
            // Creating a new DI scope allows the usage of scoped services which are valid for the total
            // lifetime of one handling process.
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                var eventDataConverter = scope.ServiceProvider.GetRequiredService<IEventDataConverter>();
                var envelopeConsumer = scope.ServiceProvider.GetRequiredService<IMessageConsumer>();

                string messageType = (string)eventData.Properties[TransportHeaderNames.MessageType];
                if (!IsKnownMessageType(messageType))
                {
                    // TODO this should be in one central location - see TypedHandling etc.

                    if (_options.UnknownMessageBehavior == UnknownMessageBehavior.ThrowException)
                    {
                        // TODO separate exception type.
                        throw new InvalidOperationException($"The message type '{messageType}' has not been configured for this consumer.");
                    }
                    else if (_options.UnknownMessageBehavior == UnknownMessageBehavior.Skip)
                    {
                        _logger.LogInformation("Skipping unknown message type {MessageType}", messageType);
                        return;
                    }
                }

                Envelope envelope = eventDataConverter.ConvertToEnvelope(eventData);

                await envelopeConsumer.ForEnvelope(envelope)
                    .SetCancellationToken(cancellation)
                    .SetRequestServices(scope.ServiceProvider)
                    .ConsumeAsync();
            }
        }

        private bool IsKnownMessageType(string messageType)
        {
            return _options.MessageTypes.Any(x => string.Equals(x.FullName, messageType, StringComparison.OrdinalIgnoreCase));
        }
    }
}