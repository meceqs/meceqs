using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Handling
{
    public class DefaultEnvelopeHandler : IEnvelopeHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHandleInvoker _handleInvoker;

        public DefaultEnvelopeHandler(IServiceProvider serviceProvider)
            : this(serviceProvider, new DefaultHandleInvoker())
        {
        }

        public DefaultEnvelopeHandler(IServiceProvider serviceProvider, IHandleInvoker handleInvoker)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));
            Check.NotNull(handleInvoker, nameof(handleInvoker));

            _serviceProvider = serviceProvider;
            _handleInvoker = handleInvoker;
        }

        public Task<TResult> HandleAsync<TMessage, TResult>(Envelope<TMessage> envelope, CancellationToken cancellation)
            where TMessage : IMessage
        {
            Check.NotNull(envelope, nameof(envelope));

            // There's no point in processing a message if the most basic values are missing.
            envelope.EnsureValid();

            var handle = _serviceProvider.GetRequiredService<IHandles<TMessage, TResult>>();

            var handleContext = new HandleContext<TMessage>(envelope, cancellation);

            // Having another component which actually calls the handler
            // allows people to use decorators that already know about the correct handler.
            return _handleInvoker.InvokeHandleAsync(handle, handleContext);
        }
    }
}