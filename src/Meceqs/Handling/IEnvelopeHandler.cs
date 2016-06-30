using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Handling
{
    public interface IEnvelopeHandler
    {
        Task<TResult> HandleAsync<TMessage, TResult>(Envelope<TMessage> envelope, CancellationToken cancellation)
            where TMessage : IMessage;
    }

    public static class EnvelopeHandlerExtensions
    {
        public static Task HandleAsync<TMessage>(this IEnvelopeHandler envelopeHandler, Envelope<TMessage> envelope, CancellationToken cancellation)
            where TMessage : IMessage
        {
            Check.NotNull(envelopeHandler, nameof(envelopeHandler));

            return envelopeHandler.HandleAsync<TMessage, VoidType>(envelope, cancellation);
        }

        public static Task HandleUntypedAsync(this IEnvelopeHandler envelopeHandler, Envelope envelope, CancellationToken cancellation)
        {
            Check.NotNull(envelopeHandler, nameof(envelopeHandler));

            // This trick with "dynamic" has "good enough" performance for now. It could probably be optimized further,
            // but it's already a lot faster than regular reflection.
            // (see "Meceqs.Tests.Performance" in test project for a benchmark test)

            var dynamicEnvelope = (dynamic)envelope;

            // Method is called via static class so that the runtime isn't confused about which HandleAsync-method to use.
            // It would throw an exception because there's also one that expects a result-type.
            return EnvelopeHandlerExtensions.HandleAsync(envelopeHandler, dynamicEnvelope, cancellation);
        }
    }
}