using System;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.Sending.TypedSend;
using Xunit;

namespace Meceqs.Tests.Sending.TypedSend
{
    public class SenderInvokerTest
    {
        private ISenderInvoker GetInvoker()
        {
            return new DefaultSenderInvoker();
        }

        private class SimpleMessageStringSender : ISender<SimpleMessage, string>
        {
            private readonly string _result;

            public SimpleMessageStringSender(string result)
            {
                _result = result;
            }

            public Task<string> SendAsync(MessageContext<SimpleMessage> context)
            {
                return Task.FromResult(_result);
            }
        }

        private class SimpleMessageSimpleResultSender : ISender<SimpleMessage, SimpleResult>
        {
            private readonly SimpleResult _result;

            public SimpleMessageSimpleResultSender(SimpleResult result)
            {
                _result = result;
            }

            public Task<SimpleResult> SendAsync(MessageContext<SimpleMessage> context)
            {
                return Task.FromResult(_result);
            }
        }

        private class SimpleMessageVoidTypeSender : ISender<SimpleMessage, VoidType>
        {
            public Task<VoidType> SendAsync(MessageContext<SimpleMessage> context)
            {
                return Task.FromResult(VoidType.Value);
            }
        }

        private MessageContext GetMessageContext<TMessage>()
            where TMessage : class, IMessage, new()
        {
            var envelope = TestObjects.Envelope<TMessage>();
            var messageContextData = new MessageContextData();

            return new MessageContext<TMessage>(envelope, messageContextData, CancellationToken.None);
        }

        [Fact]
        public async Task InvokeSendAsync_throws_if_parameters_are_missing()
        {
            // Arrange
            var invoker = GetInvoker();
            var sender = new SimpleMessageStringSender("result");
            var context = GetMessageContext<SimpleMessage>();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => invoker.InvokeSendAsync<string>(null, context));
            await Assert.ThrowsAsync<ArgumentNullException>(() => invoker.InvokeSendAsync<string>(sender, null));
        }

        [Fact]
        public async Task InvokeSendAsync_succeeds_for_SimpleMessage_and_string()
        {
            // Arrange
            var invoker = GetInvoker();
            var sender = new SimpleMessageStringSender("result");
            var context = GetMessageContext<SimpleMessage>();

            // Act
            string result = await invoker.InvokeSendAsync<string>(sender, context);

            // Assert
            Assert.Equal("result", result);
        }

        [Fact]
        public async Task InvokeSendAsync_succeeds_for_SimpleMessage_and_VoidType()
        {
            // Arrange
            var invoker = GetInvoker();
            var sender = new SimpleMessageVoidTypeSender();
            var context = GetMessageContext<SimpleMessage>();

            // Act
            VoidType result = await invoker.InvokeSendAsync<VoidType>(sender, context);

            // Assert
            Assert.Equal(VoidType.Value, result);
        }

         [Fact]
        public async Task InvokeSendAsync_succeeds_for_SimpleMessage_and_SimpleResult()
        {
            // Arrange
            var invoker = GetInvoker();
            var expectedResult = new SimpleResult();
            var sender = new SimpleMessageSimpleResultSender(expectedResult);
            var context = GetMessageContext<SimpleMessage>();

            // Act
            SimpleResult result = await invoker.InvokeSendAsync<SimpleResult>(sender, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task InvokeSendAsync_throws_if_sender_isnt_ISender()
        {
            // Arrange
            var invoker = GetInvoker();
            var sender = new SimpleMessage(); // some object other than ISender<,>
            var context = GetMessageContext<SimpleMessage>();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCastException>(() => invoker.InvokeSendAsync<int>(sender, context));
        }

        [Fact]
        public async Task InvokeSendAsync_throws_for_wrong_ResultType()
        {
            // Arrange
            var invoker = GetInvoker();
            var sender = new SimpleMessageStringSender("result");
            var context = GetMessageContext<SimpleMessage>();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCastException>(() => invoker.InvokeSendAsync<int>(sender, context));
        }

        [Fact]
        public async Task InvokeSendAsync_throws_for_wrong_MessageContext()
        {
            // Arrange
            var invoker = GetInvoker();
            var sender = new SimpleMessageStringSender("result");
            var context = GetMessageContext<SimpleCommand>();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCastException>(() => invoker.InvokeSendAsync<string>(sender, context));
        }
    }
}