using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.AzureEventHubs.Internal;
using Meceqs.Configuration;
using Meceqs.Receiving;
using Meceqs.Transport;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Meceqs.AzureEventHubs.Receiving
{
    public class DefaultEventHubReceiver : IEventHubReceiver
    {
        private readonly EventHubReceiverOptions _options;
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DefaultEventHubReceiver(
            IOptionsMonitor<EventHubReceiverOptions> optionsMonitor,
            ILoggerFactory loggerFactory,
            IServiceScopeFactory serviceScopeFactory)
        {
            Guard.NotNull(optionsMonitor, nameof(optionsMonitor));
            Guard.NotNull(loggerFactory, nameof(loggerFactory));
            Guard.NotNull(serviceScopeFactory, nameof(serviceScopeFactory));

            // TODO !!! EventHubReceiver doesn't work with named options yet.
            _options = optionsMonitor.Get(MeceqsDefaults.ReceivePipelineName);

            _logger = loggerFactory.CreateLogger<DefaultEventHubReceiver>();
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task ReceiveAsync(EventData eventData, CancellationToken cancellation)
        {
            Guard.NotNull(eventData, nameof(eventData));

            // Make sure each log message contains data about the currently processed message.
            using (_logger.EventDataScope(eventData))
            {
                _logger.ReceiveStarting(eventData);

                long startTimestamp = DateTime.UtcNow.Ticks;
                bool success = false;

                try
                {
                    await CreateScopeAndInvokeMessageReceiver(eventData, cancellation);
                    success = true;
                }
                catch (Exception ex)
                {
                    success = false;
                    _logger.ReceiveFailed(eventData, ex);
                    throw;
                }
                finally
                {
                    _logger.ReceiveFinished(success, startTimestamp, DateTime.UtcNow.Ticks);
                }
            }
        }

        private async Task CreateScopeAndInvokeMessageReceiver(EventData eventData, CancellationToken cancellation)
        {
            // Handling a message from an underlying transport is similar to handling a HTTP-request.
            // We must make sure processing of one message doesn't have an effect on other messages.
            // Creating a new DI scope allows the usage of scoped services which are valid for the total
            // lifetime of one handling process.
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                var eventDataConverter = scope.ServiceProvider.GetRequiredService<IEventDataConverter>();
                var messageReceiver = scope.ServiceProvider.GetRequiredService<IMessageReceiver>();

                string messageType = (string)eventData.Properties[TransportHeaderNames.MessageType];
                if (!IsKnownMessageType(messageType))
                {
                    // TODO this should be in one central location - see TypedHandling etc.

                    if (_options.UnknownMessageBehavior == UnknownMessageBehavior.ThrowException)
                    {
                        // TODO separate exception type.
                        throw new InvalidOperationException($"The message type '{messageType}' has not been configured for this receiver.");
                    }
                    else if (_options.UnknownMessageBehavior == UnknownMessageBehavior.Skip)
                    {
                        _logger.LogInformation("Skipping unknown message type {MessageType}", messageType);
                        return;
                    }
                }

                Envelope envelope = eventDataConverter.ConvertToEnvelope(eventData);

                await messageReceiver.ForEnvelope(envelope)
                    .SetCancellationToken(cancellation)
                    .ReceiveAsync();
            }
        }

        private bool IsKnownMessageType(string messageType)
        {
            return _options.MessageTypes.Any(x => string.Equals(x.MessageType.FullName, messageType, StringComparison.OrdinalIgnoreCase));
        }
    }
}