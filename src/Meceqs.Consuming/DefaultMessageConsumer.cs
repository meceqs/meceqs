using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Consuming
{
    public class DefaultMessageConsumer : IMessageConsumer
    {
        private readonly IConsumerResolver _consumerResolver;
        private readonly IConsumeContextFactory _consumeContextFactory;
        private readonly IConsumerInvoker _consumerInvoker;

        public DefaultMessageConsumer(IServiceProvider serviceProvider)
            : this(new DefaultConsumerResolver(serviceProvider), new DefaultConsumeContextFactory(), new DefaultConsumerInvoker())
        {
        }

        public DefaultMessageConsumer(
            IConsumerResolver consumerResolver,
            IConsumeContextFactory consumeContextFactory,
            IConsumerInvoker consumerInvoker)
        {
            if (consumerResolver == null)
                throw new ArgumentNullException(nameof(consumerResolver));

            if (consumeContextFactory == null)
                throw new ArgumentNullException(nameof(consumeContextFactory));

            if (consumerInvoker == null)
                throw new ArgumentNullException(nameof(consumerInvoker));

            _consumerResolver = consumerResolver;
            _consumeContextFactory = consumeContextFactory;
            _consumerInvoker = consumerInvoker;
        }

        public async Task<TResult> ConsumeAsync<TMessage, TResult>(
            MessageEnvelope<TMessage> envelope,
            CancellationToken cancellation = default(CancellationToken)) where TMessage : IMessage
        {
            if (envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            // Find matching handler
            var handler = _consumerResolver.Resolve<TMessage, TResult>();
            if (handler == null)
            {
                throw new InvalidOperationException($"No handler found for '{typeof(TMessage)}/{typeof(TResult)}'");
            }

            // Create context for invocation
            var handleContext = _consumeContextFactory.Create(envelope, cancellation);

            // Offload invocation to another component to allow users to use decorators
            var result = await _consumerInvoker.InvokeAsync(handler, handleContext);

            return result;
        }
    }
}