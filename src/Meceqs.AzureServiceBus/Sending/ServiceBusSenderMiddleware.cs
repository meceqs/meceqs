using System;
using System.Threading.Tasks;
using Meceqs.AzureServiceBus.Internal;
using Meceqs.Pipeline;
using Microsoft.Extensions.Options;

namespace Meceqs.AzureServiceBus.Sending
{
    public class ServiceBusSenderMiddleware : IDisposable
    {
        // TODO MessageSender lifecycle - should it be transient?

        private readonly IServiceBusMessageSender _sender;

        public ServiceBusSenderMiddleware(
            MiddlewareDelegate next,
            IOptions<ServiceBusSenderOptions> options,
            IServiceBusMessageSenderFactory senderFactory)
        {
            // "next" is not stored because this is a terminating middleware.

            Guard.NotNull(options?.Value, nameof(options));
            Guard.NotNull(senderFactory, nameof(senderFactory));

            _sender = senderFactory.CreateMessageSender(options.Value.ConnectionString);
        }

        public Task Invoke(MessageContext context, IServiceBusMessageConverter serviceBusMessageConverter)
        {
            Guard.NotNull(context, nameof(context));
            Guard.NotNull(serviceBusMessageConverter, nameof(serviceBusMessageConverter));

            var serviceBusMessage = serviceBusMessageConverter.ConvertToServiceBusMessage(context.Envelope);

            return _sender.SendAsync(serviceBusMessage);
        }

        public void Dispose()
        {
            _sender?.Close();
        }
    }
}