using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Handling
{
    public class DefaultUntypedMessageHandler : IUntypedMessageHandler
    {
        private readonly IMessageHandlingMediator _mediator;

        public DefaultUntypedMessageHandler(IMessageHandlingMediator mediator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));

            _mediator = mediator;
        }

        public async Task HandleAsync(Envelope envelope, CancellationToken cancellation)
        {
            // This trick with "dynamic" has "good enough" performance for now. It could probably be optimized further,
            // but it's already a lot faster than regular reflection.
            // (see ReflectionTest.cs in test project for a benchmark test)

            var dynamicEnvelope = (dynamic)envelope;

            // Method is called via static class so that the runtime isn't confused about which HandleAsync-method to use.
            // It would throw an exception because there's also one that expects a result-type.
            await MessageHandlingMediatorExtensions.HandleAsync(_mediator, dynamicEnvelope, CancellationToken.None);
        }
    }
}