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
            MessageDelegate next,
            IOptions<ServiceBusSenderOptions> options,
            IServiceBusMessageSenderFactory senderFactory)
        {
            // "next" is not stored because this is a terminating middleware.

            Check.NotNull(options?.Value, nameof(options));
            Check.NotNull(senderFactory, nameof(senderFactory));

            _sender = senderFactory.CreateMessageSender(options.Value.ConnectionString, options.Value.EntityPath);
        }

        public Task Invoke(MessageContext context, IBrokeredMessageConverter brokeredMessageConverter)
        {
            Check.NotNull(context, nameof(context));
            Check.NotNull(brokeredMessageConverter, nameof(brokeredMessageConverter));

            var brokeredMessage = brokeredMessageConverter.ConvertToBrokeredMessage(context.Envelope);

            return _sender.SendAsync(brokeredMessage);
        }

        public void Dispose()
        {
            _sender?.Close();
        }
    }
}