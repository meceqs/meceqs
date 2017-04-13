using System;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.AzureServiceBus.Configuration;
using Meceqs.AzureServiceBus.Internal;
using Meceqs.Receiving;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Meceqs.AzureServiceBus.Receiving
{
    public class DefaultServiceBusReceiver : IServiceBusReceiver
    {
        private readonly ServiceBusReceiverOptions _options;
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IServiceBusMessageInvoker _serviceBusMessageInvoker;

        public DefaultServiceBusReceiver(
            IOptions<ServiceBusReceiverOptions> options,
            ILoggerFactory loggerFactory,
            IServiceScopeFactory serviceScopeFactory,
            IServiceBusMessageInvoker serviceBusMessageInvoker)
        {
            Guard.NotNull(options?.Value, nameof(options));
            Guard.NotNull(loggerFactory, nameof(loggerFactory));
            Guard.NotNull(serviceScopeFactory, nameof(serviceScopeFactory));
            Guard.NotNull(serviceBusMessageInvoker, nameof(serviceBusMessageInvoker));

            _options = options.Value;
            _logger = loggerFactory.CreateLogger<DefaultServiceBusReceiver>();
            _serviceScopeFactory = serviceScopeFactory;
            _serviceBusMessageInvoker = serviceBusMessageInvoker;
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellation)
        {
            Guard.NotNull(message, nameof(message));

            // Make sure each log message contains data about the currently processed message.
            using (_logger.ServiceBusMessageScope(message))
            {
                _logger.ReceiveStarting(message);

                long startTimestamp = DateTime.UtcNow.Ticks;
                bool success = false;

                try
                {
                    await ResolveServicesAndInvokeReceiver(message, cancellation);

                    await _serviceBusMessageInvoker.CompleteAsync(message);
                    success = true;
                }
                catch (Exception ex)
                {
                    success = false;

                    _logger.ReceiveFailed(message, ex);

                    await _serviceBusMessageInvoker.AbandonAsync(message);

                    // TODO @cweiss !!! remove this!
                    throw;
                }
                finally
                {
                    _logger.ReceiveFinished(success, startTimestamp, DateTime.UtcNow.Ticks);
                }
            }
        }

        private async Task ResolveServicesAndInvokeReceiver(Message message, CancellationToken cancellation)
        {
            // Handling a message from an underlying transport is similar to handling a HTTP-request.
            // We must make sure processing of one message doesn't have an effect on other messages.
            // Creating a new DI scope allows the usage of scoped services which are valid for the total
            // lifetime of one handling process.
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                var serviceBusMessageConverter = scope.ServiceProvider.GetRequiredService<IServiceBusMessageConverter>();
                var messageReceiver = scope.ServiceProvider.GetRequiredService<IMessageReceiver>();

                Envelope envelope = serviceBusMessageConverter.ConvertToEnvelope(message);

                await messageReceiver.ForEnvelope(envelope)
                    .SetCancellationToken(cancellation)
                    .ReceiveAsync();
            }
        }
    }
}