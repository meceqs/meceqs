using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Handling
{
    public class DefaultMessageHandlingMediator : IMessageHandlingMediator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHandlerInvoker _handlerInvoker;

        public DefaultMessageHandlingMediator(IServiceProvider serviceProvider)
            : this(serviceProvider, new DefaultHandlerInvoker())
        {
        }

        public DefaultMessageHandlingMediator(IServiceProvider serviceProvider, IHandlerInvoker handlerInvoker)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            if (handlerInvoker == null)
                throw new ArgumentNullException(nameof(handlerInvoker));

            _serviceProvider = serviceProvider;
            _handlerInvoker = handlerInvoker;
        }

        public Task<TResult> HandleAsync<TMessage, TResult>(
            Envelope<TMessage> envelope,
            CancellationToken cancellation = default(CancellationToken)) where TMessage : IMessage
        {
            if (envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            // There's no point in processing a message if the most basic values are missing.
            envelope.EnsureValid();

            var handler = _serviceProvider.GetRequiredService<IHandles<TMessage, TResult>>();

            var handleContext = new HandleContext<TMessage>(envelope, cancellation);

            // Having another component which actually calls the handler
            // allows people to use decorators that already know about the correct handler.
            return _handlerInvoker.InvokeHandleAsync(handler, handleContext);
        }
    }
}