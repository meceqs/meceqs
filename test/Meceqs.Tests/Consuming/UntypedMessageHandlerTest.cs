using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Meceqs.Tests.Handling
{
    public class UntypedMessageHandlerTest
    {
        public class CallbackEnvelopeHandler : IEnvelopeHandler
        {
            private readonly Action<Envelope, Type, Type> _callback;

            public CallbackEnvelopeHandler(Action<Envelope, Type, Type> callback)
            {
                _callback = callback;
            }

            public async Task<TResult> HandleAsync<TMessage, TResult>(Envelope<TMessage> envelope, CancellationToken cancellation)
                where TMessage : IMessage
            {
                _callback(envelope, typeof(TMessage), typeof(TResult));

                return await Task.FromResult<TResult>(default(TResult));
            }
        }

        [Fact]
        public async Task Calls_mediator_with_correct_types()
        {
            Envelope envelope = TestObjects.Envelope<SimpleMessage>();

            bool mediatorCalled = false;

            var envelopeHandler = new CallbackEnvelopeHandler((actualEnvelope, messageType, returnType) =>
            {
                mediatorCalled = true;

                Assert.Equal(typeof(SimpleMessage), messageType);
                Assert.Equal(typeof(VoidType), returnType);
                Assert.Same(envelope, actualEnvelope);
            });

            await envelopeHandler.HandleUntypedAsync(envelope, CancellationToken.None);

            Assert.True(mediatorCalled);
        }
    }
}