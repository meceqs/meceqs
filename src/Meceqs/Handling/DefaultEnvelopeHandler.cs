using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Handling
{
    public class DefaultEnvelopeHandler : IEnvelopeHandler
    {
        private readonly IHandlerFactory _handlerFactory;
        private readonly IHandlerInvoker _handlerInvoker;

        public DefaultEnvelopeHandler(IHandlerFactory handlerFactory, IHandlerInvoker handlerInvoker)
        {
            Check.NotNull(handlerFactory, nameof(handlerFactory));
            Check.NotNull(handlerInvoker, nameof(handlerInvoker));

            _handlerFactory = handlerFactory;
            _handlerInvoker = handlerInvoker;
        }

        public Task<TResult> HandleAsync<TMessage, TResult>(Envelope<TMessage> envelope, CancellationToken cancellation)
            where TMessage : IMessage
        {
            Check.NotNull(envelope, nameof(envelope));

            // There's no point in processing a message if the most basic values are missing.
            envelope.EnsureValid();

            var handler = _handlerFactory.CreateHandler<TMessage, TResult>();
            if (handler == null)
            {
                throw new InvalidOperationException($"No handler found for '{typeof(TMessage)}/{typeof(TResult)}'");
            }

            var messageContext = new MessageContext<TMessage>(envelope, null, cancellation);

            // Having another component which actually calls the handler
            // allows people to use decorators that already know about the correct handler.
            return _handlerInvoker.InvokeHandleAsync(handler, messageContext);
        }
    }
}