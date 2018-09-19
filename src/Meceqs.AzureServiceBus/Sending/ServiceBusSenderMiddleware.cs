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

        private readonly IOptionsMonitor<ServiceBusSenderOptions> _optionsMonitor;
        private readonly IServiceBusMessageSenderFactory _senderFactory;

        private readonly object _lock = new object();
        private IServiceBusMessageSender _sender;

        public ServiceBusSenderMiddleware(
            MiddlewareDelegate next,
            IOptionsMonitor<ServiceBusSenderOptions> optionsMonitor,
            IServiceBusMessageSenderFactory senderFactory)
        {
            // "next" is not stored because this is a terminating middleware.

            Guard.NotNull(optionsMonitor, nameof(optionsMonitor));
            Guard.NotNull(senderFactory, nameof(senderFactory));

            _optionsMonitor = optionsMonitor;
            _senderFactory = senderFactory;
        }

        public Task Invoke(MessageContext context, IServiceBusMessageConverter serviceBusMessageConverter)
        {
            Guard.NotNull(context, nameof(context));
            Guard.NotNull(serviceBusMessageConverter, nameof(serviceBusMessageConverter));

            var serviceBusMessage = serviceBusMessageConverter.ConvertToServiceBusMessage(context.Envelope);

            var sender = GetOrCreateSender(context.PipelineName);

            return _sender.SendAsync(serviceBusMessage);
        }

        public void Dispose()
        {
            _sender?.Close();
        }

        private IServiceBusMessageSender GetOrCreateSender(string pipelineName)
        {
            if (_sender != null)
                return _sender;

            lock (_lock)
            {
                if (_sender != null)
                    return _sender;

                var options = _optionsMonitor.Get(pipelineName);
                _sender = _senderFactory.CreateMessageSender(options.ConnectionString);
                return _sender;
            }
        }
    }
}
