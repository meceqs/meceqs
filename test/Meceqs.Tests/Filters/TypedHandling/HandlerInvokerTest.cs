using System;
using System.Threading.Tasks;
using Meceqs.Filters.TypedHandling;
using Meceqs.Filters.TypedHandling.Internal;
using Meceqs.Pipeline;
using Xunit;

namespace Meceqs.Tests.Filters.TypedHandling
{
    public class SenderInvokerTest
    {
        private IHandlerInvoker GetInvoker()
        {
            return new DefaultHandlerInvoker();
        }

        private class SimpleMessageStringHandler : IHandles<SimpleMessage, string>
        {
            private readonly string _result;

            public SimpleMessageStringHandler(string result)
            {
                _result = result;
            }

            public Task<string> HandleAsync(HandleContext<SimpleMessage> context)
            {
                return Task.FromResult(_result);
            }
        }

        private class SimpleMessageSimpleResultHandler : IHandles<SimpleMessage, SimpleResult>
        {
            private readonly SimpleResult _result;

            public SimpleMessageSimpleResultHandler(SimpleResult result)
            {
                _result = result;
            }

            public Task<SimpleResult> HandleAsync(HandleContext<SimpleMessage> context)
            {
                return Task.FromResult(_result);
            }
        }

        private class SimpleMessageVoidTypeHandler : IHandles<SimpleMessage, VoidType>
        {
            public Task<VoidType> HandleAsync(HandleContext<SimpleMessage> context)
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

        private HandleContext<TMessage> GetHandleContext<TMessage, TResult>(FilterContext<TMessage> filterContext = null)
            where TMessage : class, IMessage, new()
        {
            filterContext = filterContext ?? GetFilterContext<TMessage, TResult>();

            return new HandleContext<TMessage>(filterContext);
        }

        [Fact]
        public async Task Throws_if_parameters_are_missing()
        {
            // Arrange
            var invoker = GetInvoker();
            var sender = new SimpleMessageStringHandler("result");
            var context = GetHandleContext<SimpleMessage, string>();
            var resultType = typeof(string);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => invoker.InvokeHandleAsync(null, context, resultType));
            await Assert.ThrowsAsync<ArgumentNullException>(() => invoker.InvokeHandleAsync(sender, null, resultType));
            await Assert.ThrowsAsync<ArgumentNullException>(() => invoker.InvokeHandleAsync(sender, context, null));
        }

        [Fact]
        public async Task Succeeds_for_SimpleMessage_and_string()
        {
            // Arrange
            var invoker = GetInvoker();
            var sender = new SimpleMessageStringHandler("result");
            var context = GetHandleContext<SimpleMessage, string>();
            var resultType = typeof(string);

            // Act
            string result = (string)await invoker.InvokeHandleAsync(sender, context, resultType);

            // Assert
            Assert.Equal("result", result);
        }

        [Fact]
        public async Task Succeeds_for_SimpleMessage_and_VoidType()
        {
            // Arrange
            var invoker = GetInvoker();
            var sender = new SimpleMessageVoidTypeHandler();
            var context = GetHandleContext<SimpleMessage, VoidType>();
            var resultType = typeof(VoidType);

            // Act
            VoidType result = (VoidType)await invoker.InvokeHandleAsync(sender, context, resultType);

            // Assert
            Assert.Equal(VoidType.Value, result);
        }

        [Fact]
        public async Task Succeeds_for_SimpleMessage_and_SimpleResult()
        {
            // Arrange
            var invoker = GetInvoker();
            var expectedResult = new SimpleResult();
            var sender = new SimpleMessageSimpleResultHandler(expectedResult);
            var context = GetHandleContext<SimpleMessage, SimpleResult>();
            var resultType = typeof(SimpleResult);

            // Act
            SimpleResult result = (SimpleResult)await invoker.InvokeHandleAsync(sender, context, resultType);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task Throws_if_handler_isnt_IHandles()
        {
            // Arrange
            var invoker = GetInvoker();
            var sender = new SimpleMessage(); // some object other than IHandles<,>
            var context = GetHandleContext<SimpleMessage, int>();
            var resultType = typeof(int);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCastException>(() => invoker.InvokeHandleAsync(sender, context, resultType));
        }

        [Fact]
        public async Task Throws_for_wrong_ResultType()
        {
            // Arrange
            var invoker = GetInvoker();
            var sender = new SimpleMessageStringHandler("result");
            var context = GetHandleContext<SimpleMessage, int>();
            var resultType = typeof(int);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCastException>(() => invoker.InvokeHandleAsync(sender, context, resultType));
        }

        [Fact]
        public async Task Throws_for_wrong_Context()
        {
            // Arrange
            var invoker = GetInvoker();
            var sender = new SimpleMessageStringHandler("result");
            var context = GetHandleContext<SimpleCommand, string>();
            var resultType = typeof(string);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCastException>(() => invoker.InvokeHandleAsync(sender, context, resultType));
        }
    }
}