using System;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.Handling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureServiceBus
{
    public class BrokeredMessageHandler : IBrokeredMessageHandler
    {
        private readonly ILogger _logger;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public BrokeredMessageHandler(ILoggerFactory loggerFactory, IServiceScopeFactory serviceScopeFactory)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            if (serviceScopeFactory == null)
                throw new ArgumentNullException(nameof(serviceScopeFactory));

            _logger = loggerFactory.CreateLogger<BrokeredMessageHandler>();
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task HandleAsync(BrokeredMessage brokeredMessage, CancellationToken cancellation)
        {
            if (brokeredMessage == null)
                throw new ArgumentNullException(nameof(brokeredMessage));

            using (_logger.BrokeredMessageScope(brokeredMessage))
            {
                _logger.HandleStarting(brokeredMessage);

                long startTimestamp = DateTime.UtcNow.Ticks;
                bool success = false;

                try
                {
                    await ResolveServicesAndInvokeMediator(brokeredMessage, cancellation);

                    await brokeredMessage.CompleteAsync();
                    success = true;
                }
                catch (Exception ex)
                {
                    success = false;
                    _logger.LogError(0, ex, "Exception occured while handling message");

                    await brokeredMessage.AbandonAsync();
                }
                finally
                {
                    _logger.HandleFinished(success, startTimestamp, DateTime.UtcNow.Ticks);
                }
            }
        }

        private async Task ResolveServicesAndInvokeMediator(BrokeredMessage brokeredMessage, CancellationToken cancellation)
        {
            // Handling a message from an underlying transport is similar to handling a HTTP-request.
            // We must make sure, processing of one message doesn't have an effect on other messages.
            // Creating a new DI scope allows the usage of scoped services which are valid for the total
            // lifetime of one handling process.
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                var brokeredMessageConverter = scope.ServiceProvider.GetRequiredService<IBrokeredMessageConverter>();
                var handlingMediator = scope.ServiceProvider.GetRequiredService<IEnvelopeHandler>();

                Envelope envelope = brokeredMessageConverter.ConvertToEnvelope(brokeredMessage);

                await handlingMediator.HandleUntypedAsync(envelope, cancellation);
            }
        }
    }
}