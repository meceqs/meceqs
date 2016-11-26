using System;
using System.Threading.Tasks;
using Meceqs.Amqp.Configuration;
using Meceqs.Amqp.Internal;
using Meceqs.Pipeline;
using Microsoft.Extensions.Options;

namespace Meceqs.Amqp.Sending
{
    public class AmqpSenderMiddleware : IDisposable
    {
        private readonly AmqpSenderOptions _options;
        private readonly Lazy<ISenderLink> _senderLink;
        private readonly IAmqpMessageConverter _messageConverter;

        public AmqpSenderMiddleware(
            MiddlewareDelegate next,
            IOptions<AmqpSenderOptions> options,
            ISenderLinkFactory senderLinkFactory,
            IAmqpMessageConverter messageConverter)
        {
            Check.NotNull(options?.Value, nameof(options));
            Check.NotNull(options.Value.Address, $"{nameof(options)}.{nameof(options.Value.Address)}");
            Check.NotNull(senderLinkFactory, nameof(senderLinkFactory));
            Check.NotNull(messageConverter, nameof(messageConverter));

            _options = options.Value;
            _messageConverter = messageConverter;

            _senderLink = new Lazy<ISenderLink>(() =>
            {
                return senderLinkFactory.CreateSenderLink(_options.Address, _options.SenderLinkName, _options.SenderLinkAddress);
            });
        }

        public async Task Invoke(MessageContext context)
        {
            var message = _messageConverter.ConvertToAmqpMessage(context.Envelope);

            await _senderLink.Value.SendAsync(message);
        }

        public void Dispose()
        {
            if (_senderLink?.IsValueCreated == true)
            {
                _senderLink.Value?.Dispose();
            }
        }
    }
}