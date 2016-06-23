using System;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.Handling;
using Xunit;

namespace Meceqs.Tests.Handling
{
    public class UntypedMessageHandlerTest
    {
        public class CallbackMediator : IMessageHandlingMediator
        {
            private readonly Action<Envelope, Type, Type> _callback;

            public CallbackMediator(Action<Envelope, Type, Type> callback)
            {
                _callback = callback;
            }

            public async Task<TResult> HandleAsync<TMessage, TResult>(Envelope<TMessage> envelope, CancellationToken cancellation) where TMessage : IMessage
            {
                _callback(envelope, typeof(TMessage), typeof(TResult));

                return await Task.FromResult<TResult>(default(TResult));
            }
        }

        private IUntypedMessageHandler GetUntypedMessageHandler(Action<Envelope, Type, Type> callback)
        {
            return new DefaultUntypedMessageHandler(new CallbackMediator(callback));
        }

        [Fact]
        public async Task Calls_mediator_with_correct_types()
        {
            var envelope = TestObjects.Envelope<SimpleMessage>();

            bool mediatorCalled = false;

            var untypedHandler = GetUntypedMessageHandler((actualEnvelope, messageType, returnType) =>
            {
                mediatorCalled = true;

                Assert.Equal(typeof(SimpleMessage), messageType);
                Assert.Equal(typeof(VoidType), returnType);
                Assert.Same(envelope, actualEnvelope);
            });

            await untypedHandler.HandleAsync(envelope, CancellationToken.None);

            Assert.True(mediatorCalled);
        }
    }
}