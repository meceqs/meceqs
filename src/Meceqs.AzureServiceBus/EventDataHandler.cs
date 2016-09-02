using System;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.Consuming;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureServiceBus
{
    public class EventDataHandler : IEventDataHandler
    {
        private readonly ILogger _logger;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EventDataHandler(ILoggerFactory loggerFactory, IServiceScopeFactory serviceScopeFactory)
        {
            Check.NotNull(loggerFactory, nameof(loggerFactory));
            Check.NotNull(serviceScopeFactory, nameof(serviceScopeFactory));

            _logger = loggerFactory.CreateLogger<EventDataHandler>();
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task HandleAsync(EventData eventData, CancellationToken cancellation)
        {
            Check.NotNull(eventData, nameof(eventData));

            // Make sure each log message contains data about the currently processed message.
            using (_logger.EventDataScope(eventData))
            {
                _logger.HandleStarting(eventData);

                long startTimestamp = DateTime.UtcNow.Ticks;
                bool success = false;

                try
                {
                    await ResolveServicesAndInvokeConsumer(eventData, cancellation);
                    success = true;
                }
                catch (Exception ex)
                {
                    success = false;
                    _logger.HandleFailed(eventData, ex);
                    throw;
                }
                finally
                {
                    _logger.HandleEventDataFinished(success, startTimestamp, DateTime.UtcNow.Ticks);
                }
            }
        }

        private async Task ResolveServicesAndInvokeConsumer(EventData eventData, CancellationToken cancellation)
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