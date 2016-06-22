using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Handling
{
    public class DefaultMessageHandlingMediator : IMessageHandlingMediator
    {
        private readonly IHandlerResolver _handlerResolver;
        private readonly IHandlerInvoker _handlerInvoker;

        public DefaultMessageHandlingMediator(IServiceProvider serviceProvider)
            : this(new DefaultHandlerResolver(serviceProvider), new DefaultHandlerInvoker())
        {
        }

        public DefaultMessageHandlingMediator(IHandlerResolver handlerResolver, IHandlerInvoker handlerInvoker)
        {
            if (handlerResolver == null)
                throw new ArgumentNullException(nameof(handlerResolver));

            if (handlerInvoker == null)
                throw new ArgumentNullException(nameof(handlerInvoker));

            _handlerResolver = handlerResolver;
            _handlerInvoker = handlerInvoker;
        }

        public async Task<TResult> HandleAsync<TMessage, TResult>(
            Envelope<TMessage> envelope,
            CancellationToken cancellation = default(CancellationToken)) where TMessage : IMessage
        {
            if (envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            // There's no point in processing a message if the most basic values are missing.
            envelope.EnsureValid();

            var handler = _handlerResolver.Resolve<TMessage, TResult>();
            if (handler == null)
            {
                throw new InvalidOperationException($"No handler found for '{typeof(TMessage)}/{typeof(TResult)}'");
            }

            var handleContext = new HandleContext<TMessage>(envelope, cancellation);

            // Having another component which actually calls the handler
            // allows people to use decorators that already know about the correct handler.
            var result = await _handlerInvoker.InvokeHandleAsync(handler, handleContext);

            return result;
        }
    }
}