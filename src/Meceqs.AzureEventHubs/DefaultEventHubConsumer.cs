using System;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.AzureEventHubs.Internal;
using Meceqs.Consuming;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureEventHubs
{
    public class DefaultEventHubConsumer : IEventHubConsumer
    {
        private readonly ILogger _logger;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DefaultEventHubConsumer(ILoggerFactory loggerFactory, IServiceScopeFactory serviceScopeFactory)
        {
            Check.NotNull(loggerFactory, nameof(loggerFactory));
            Check.NotNull(serviceScopeFactory, nameof(serviceScopeFactory));

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

                Envelope envelope = eventDataConverter.ConvertToEnvelope(eventData);

                await envelopeConsumer.ForEnvelope(envelope)
                    .SetCancellationToken(cancellation)
                    .ConsumeAsync();
            }
        }
    }
}