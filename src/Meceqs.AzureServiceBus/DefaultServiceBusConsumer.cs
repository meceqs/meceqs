using System;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.AzureServiceBus.Internal;
using Meceqs.Consuming;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureServiceBus
{
    public class DefaultServiceBusConsumer : IServiceBusConsumer
    {
        private readonly ILogger _logger;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DefaultServiceBusConsumer(ILoggerFactory loggerFactory, IServiceScopeFactory serviceScopeFactory)
        {
            Check.NotNull(loggerFactory, nameof(loggerFactory));
            Check.NotNull(serviceScopeFactory, nameof(serviceScopeFactory));

            _logger = loggerFactory.CreateLogger<DefaultServiceBusConsumer>();
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task ConsumeAsync(BrokeredMessage brokeredMessage, CancellationToken cancellation)
        {
            Check.NotNull(brokeredMessage, nameof(brokeredMessage));

            // Make sure each log message contains data about the currently processed message.
            using (_logger.BrokeredMessageScope(brokeredMessage))
            {
                _logger.ConsumeStarting(brokeredMessage);

                long startTimestamp = DateTime.UtcNow.Ticks;
                bool success = false;

                try
                {
                    await ResolveServicesAndInvokeConsumer(brokeredMessage, cancellation);

                    await brokeredMessage.CompleteAsync();
                    success = true;
                }
                catch (Exception ex)
                {
                    success = false;

                    _logger.ConsumeFailed(brokeredMessage, ex);

                    await brokeredMessage.AbandonAsync();
                }
                finally
                {
                    _logger.ConsumeFinished(success, startTimestamp, DateTime.UtcNow.Ticks);
                }
            }
        }

        private async Task ResolveServicesAndInvokeConsumer(BrokeredMessage brokeredMessage, CancellationToken cancellation)
        {
            // Handling a message from an underlying transport is similar to handling a HTTP-request.
            // We must make sure processing of one message doesn't have an effect on other messages.
            // Creating a new DI scope allows the usage of scoped services which are valid for the total
            // lifetime of one handling process.
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                var brokeredMessageConverter = scope.ServiceProvider.GetRequiredService<IBrokeredMessageConverter>();
                var envelopeConsumer = scope.ServiceProvider.GetRequiredService<IMessageConsumer>();

                Envelope envelope = brokeredMessageConverter.ConvertToEnvelope(brokeredMessage);

                await envelopeConsumer.ForEnvelope(envelope)
                    .SetCancellationToken(cancellation)
                    .ConsumeAsync();
            }
        }
    }
}