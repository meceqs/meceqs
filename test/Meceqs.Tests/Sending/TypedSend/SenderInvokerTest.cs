using System;
using System.Threading.Tasks;
using Meceqs.Pipeline;
using Xunit;

namespace Meceqs.Tests.Sending.TypedSend
{
    public class SenderInvokerTest
    {
        private IHandlerInvoker GetInvoker()
        {
            return new DefaultHandlerInvoker();
        }

        private class SimpleMessageStringSender : IHandles<SimpleMessage, string>
        {
            private readonly string _result;

            public SimpleMessageStringSender(string result)
            {
                _result = result;
            }

            public Task<string> SendAsync(HandleContext<SimpleMessage> context)
            {
                return Task.FromResult(_result);
            }
        }

        private class SimpleMessageSimpleResultSender : IHandles<SimpleMessage, SimpleResult>
        {
            private readonly SimpleResult _result;

            public SimpleMessageSimpleResultSender(SimpleResult result)
            {
                _result = result;
            }

            public Task<SimpleResult> SendAsync(HandleContext<SimpleMessage> context)
            {
                return Task.FromResult(_result);
            }
        }

        private class SimpleMessageVoidTypeSender : IHandles<SimpleMessage, VoidType>
        {
            public Task<VoidType> SendAsync(HandleContext<SimpleMessage> context)
            {
                return Task.FromResult(VoidType.Value);
            }
        }

        private FilterContext<TMessage> GetFilterContext<TMessage, TResult>()
            where TMessage : class, IMessage, new()
        {
            var envelope = TestObjects.Envelope<TMessage>();

            var filterContext = new FilterContext<TMessage>(envelope);
            filterContext.ExpectedResultType = typeof(TResult);

            return filterContext;
        }

        private HandleContext<TMessage> GetSendContext<TMessage, TResult>(FilterContext<TMessage> filterContext = null)
            where TMessage : class, IMessage, new()
        {
            filterContext = filterContext ?? GetFilterContext<TMessage, TResult>();

            var sendContext = new HandleContext<TMessage>(filterContext);
            return sendContext;
        }

        [Fact]
        public async Task InvokeSendAsync_throws_if_parameters_are_missing()
        {
            // Arrange
            var invoker = GetInvoker();
            var sender = new SimpleMessageStringSender("result");
            var context = GetSendContext<SimpleMessage, string>();
            var messageType = context.Message.GetType();
            var resultType = typeof(string);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => invoker.InvokeHandleAsync(null, context, messageType, resultType));
            await Assert.ThrowsAsync<ArgumentNullException>(() => invoker.InvokeHandleAsync(sender, null, messageType, resultType));
            await Assert.ThrowsAsync<ArgumentNullException>(() => invoker.InvokeHandleAsync(sender, context, null, resultType));
            await Assert.ThrowsAsync<ArgumentNullException>(() => invoker.InvokeHandleAsync(sender, context, messageType, null));
        }

        [Fact]
        public async Task InvokeSendAsync_succeeds_for_SimpleMessage_and_string()
        {
            // Arrange
            var invoker = GetInvoker();
            var sender = new SimpleMessageStringSender("result");
            var context = GetSendContext<SimpleMessage, string>();
            var messageType = context.Message.GetType();
            var resultType = typeof(string);

            // Act
            string result = (string)await invoker.InvokeHandleAsync(sender, context, messageType, resultType);

            // Assert
            Assert.Equal("result", result);
        }

        [Fact]
        public async Task InvokeSendAsync_succeeds_for_SimpleMessage_and_VoidType()
        {
            // Arrange
            var invoker = GetInvoker();
            var sender = new SimpleMessageVoidTypeSender();
            var context = GetSendContext<SimpleMessage, VoidType>();
            var messageType = context.Message.GetType();
            var resultType = typeof(VoidType);

            // Act
            VoidType result = (VoidType)await invoker.InvokeHandleAsync(sender, context, messageType, resultType);

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
            var context = GetSendContext<SimpleMessage, SimpleResult>();
            var messageType = context.Message.GetType();
            var resultType = typeof(SimpleResult);

            // Act
            SimpleResult result = (SimpleResult)await invoker.InvokeHandleAsync(sender, context, messageType, resultType);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task InvokeSendAsync_throws_if_sender_isnt_ISender()
        {
            // Arrange
            var invoker = GetInvoker();
            var sender = new SimpleMessage(); // some object other than ISender<,>
            var context = GetSendContext<SimpleMessage, int>();
            var messageType = context.Message.GetType();
            var resultType = typeof(int);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCastException>(() => invoker.InvokeHandleAsync(sender, context, messageType, resultType));
        }

        [Fact]
        public async Task InvokeSendAsync_throws_for_wrong_ResultType()
        {
            // Arrange
            var invoker = GetInvoker();
            var sender = new SimpleMessageStringSender("result");
            var context = GetSendContext<SimpleMessage, int>();
            var messageType = context.Message.GetType();
            var resultType = typeof(int);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCastException>(() => invoker.InvokeHandleAsync(sender, context, messageType, resultType));
        }

        [Fact]
        public async Task InvokeSendAsync_throws_for_wrong_Context()
        {
            // Arrange
            var invoker = GetInvoker();
            var sender = new SimpleMessageStringSender("result");
            var context = GetSendContext<SimpleCommand, string>();
            var messageType = context.Message.GetType();
            var resultType = typeof(string);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCastException>(() => invoker.InvokeHandleAsync(sender, context, messageType,resultType));
        }
    }
}