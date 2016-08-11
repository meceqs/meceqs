using System;
using System.Threading.Tasks;
using Meceqs.Filters.TypedHandling;
using Meceqs.Filters.TypedHandling.Internal;
using Meceqs.Pipeline;
using Xunit;

namespace Meceqs.Tests.Filters.TypedHandling
{
    public class HandlerInvokerTest
    {
        private IHandlerInvoker GetInvoker()
        {
            return new DefaultHandlerInvoker();
        }

        private FilterContext<TMessage> GetFilterContext<TMessage>(Type resultType)
            where TMessage : class, IMessage, new()
        {
            var envelope = TestObjects.Envelope<TMessage>();

            var filterContext = new FilterContext<TMessage>(envelope);
            filterContext.ExpectedResultType = resultType;

            return filterContext;
        }

        private HandleContext<TMessage> GetHandleContext<TMessage>(Type resultType, FilterContext<TMessage> filterContext = null)
            where TMessage : class, IMessage, new()
        {
            filterContext = filterContext ?? GetFilterContext<TMessage>(resultType);

            return new HandleContext<TMessage>(filterContext);
        }

        [Fact]
        public async Task Throws_if_parameters_are_missing()
        {
            // Arrange
            var invoker = GetInvoker();
            var handler = new SimpleMessageIntHandler(1);
            var resultType = typeof(int);
            var context = GetHandleContext<SimpleMessage>(resultType);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => invoker.InvokeHandleAsync(null, context, resultType));
            await Assert.ThrowsAsync<ArgumentNullException>(() => invoker.InvokeHandleAsync(handler, null, resultType));
        }

        [Fact]
        public async Task Succeeds_for_SimpleMessage_and_int()
        {
            // Arrange
            var invoker = GetInvoker();
            var handler = new SimpleMessageIntHandler(1);
            var resultType = typeof(int);
            var context = GetHandleContext<SimpleMessage>(resultType);

            // Act
            int result = (int)await invoker.InvokeHandleAsync(handler, context, resultType);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task Succeeds_for_SimpleMessage_without_result()
        {
            // Arrange
            var invoker = GetInvoker();
            bool handlerCalled = false;
            var handler = new SimpleMessageNoResultHandler(() => handlerCalled = true);
            Type resultType = null;
            var context = GetHandleContext<SimpleMessage>(resultType);

            // Act
            object result = await invoker.InvokeHandleAsync(handler, context, resultType);

            // Assert
            Assert.True(handlerCalled);
            Assert.Null(result);
        }

        [Fact]
        public async Task Succeeds_for_SimpleMessage_and_SimpleResult()
        {
            // Arrange
            var invoker = GetInvoker();
            var expectedResult = new SimpleResult();
            var handler = new SimpleMessageSimpleResultHandler(expectedResult);
            var resultType = typeof(SimpleResult);
            var context = GetHandleContext<SimpleMessage>(resultType);

            // Act
            SimpleResult result = (SimpleResult)await invoker.InvokeHandleAsync(handler, context, resultType);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task Throws_for_wrong_ResultType()
        {
            // Arrange
            var invoker = GetInvoker();
            var handler = new SimpleMessageIntHandler();
            var resultType = typeof(string);
            var context = GetHandleContext<SimpleMessage>(resultType);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCastException>(() => invoker.InvokeHandleAsync(handler, context, resultType));
        }

        [Fact]
        public async Task Throws_for_wrong_Context()
        {
            // Arrange
            var invoker = GetInvoker();
            var handler = new SimpleMessageIntHandler();
            var resultType = typeof(int);
            var context = GetHandleContext<SimpleCommand>(resultType);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCastException>(() => invoker.InvokeHandleAsync(handler, context, resultType));
        }
    }
}