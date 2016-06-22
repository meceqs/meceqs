using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Handling
{
    public class DefaultMessageHandlingMediator : IMessageHandlingMediator
    {
        private readonly IHandlerResolver _handlerResolver;
        private readonly IHandleContextFactory _handleContextFactory;
        private readonly IHandlerInvoker _handlerInvoker;

        public DefaultMessageHandlingMediator(IServiceProvider serviceProvider)
            : this(new DefaultHandlerResolver(serviceProvider), new DefaultHandleContextFactory(), new DefaultHandlerInvoker())
        {
        }

        public DefaultMessageHandlingMediator(
            IHandlerResolver handlerResolver,
            IHandleContextFactory handleContextFactory,
            IHandlerInvoker handlerInvoker)
        {
            if (handlerResolver == null)
                throw new ArgumentNullException(nameof(handlerResolver));

            if (handleContextFactory == null)
                throw new ArgumentNullException(nameof(handleContextFactory));

            if (handlerInvoker == null)
                throw new ArgumentNullException(nameof(handlerInvoker));

            _handlerResolver = handlerResolver;
            _handleContextFactory = handleContextFactory;
            _handlerInvoker = handlerInvoker;
        }

        public async Task<TResult> HandleAsync<TMessage, TResult>(
            MessageEnvelope<TMessage> envelope,
            CancellationToken cancellation = default(CancellationToken)) where TMessage : IMessage
        {
            if (envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            // Find matching handler
            var handler = _handlerResolver.Resolve<TMessage, TResult>();
            if (handler == null)
            {
                throw new InvalidOperationException($"No handler found for '{typeof(TMessage)}/{typeof(TResult)}'");
            }

            // Create context for invocation
            var handleContext = _handleContextFactory.Create(envelope, cancellation);

            // Offload invocation to another component to allow users to use decorators
            var result = await _handlerInvoker.InvokeAsync(handler, handleContext);

            return result;
        }
    }
}