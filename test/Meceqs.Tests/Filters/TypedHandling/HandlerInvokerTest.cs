using System;
using System.Threading.Tasks;
using Meceqs.TypedHandling.Internal;
using Shouldly;
using Xunit;

namespace Meceqs.Tests.Middleware.TypedHandling
{
    public class HandlerInvokerTest
    {
        private IHandlerInvoker GetInvoker()
        {
            return new DefaultHandlerInvoker();
        }

        [Fact]
        public async Task Throws_if_parameters_are_missing()
        {
            // Arrange
            var invoker = GetInvoker();
            var handler = new SimpleMessageIntHandler(1);
            var resultType = typeof(int);
            var context = TestObjects.HandleContext<SimpleMessage>(resultType);

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
            var context = TestObjects.HandleContext<SimpleMessage>(resultType);

            // Act
            int result = (int)await invoker.InvokeHandleAsync(handler, context, resultType);

            // Assert
            result.ShouldBe(1);
        }

        [Fact]
        public async Task Succeeds_for_SimpleMessage_without_result()
        {
            // Arrange
            var invoker = GetInvoker();
            bool handlerCalled = false;
            var handler = new SimpleMessageNoResultHandler(() => handlerCalled = true);
            Type resultType = null;
            var context = TestObjects.HandleContext<SimpleMessage>(resultType);

            // Act
            object result = await invoker.InvokeHandleAsync(handler, context, resultType);

            // Assert
            handlerCalled.ShouldBeTrue();
            result.ShouldBeNull();
        }

        [Fact]
        public async Task Succeeds_for_SimpleMessage_and_SimpleResult()
        {
            // Arrange
            var invoker = GetInvoker();
            var expectedResult = new SimpleResult();
            var handler = new SimpleMessageSimpleResultHandler(expectedResult);
            var resultType = typeof(SimpleResult);
            var context = TestObjects.HandleContext<SimpleMessage>(resultType);

            // Act
            SimpleResult result = (SimpleResult)await invoker.InvokeHandleAsync(handler, context, resultType);

            // Assert
            result.ShouldBe(expectedResult);
        }

        [Fact]
        public async Task Throws_for_wrong_ResultType()
        {
            // Arrange
            var invoker = GetInvoker();
            var handler = new SimpleMessageIntHandler();
            var resultType = typeof(string);
            var context = TestObjects.HandleContext<SimpleMessage>(resultType);

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
            var context = TestObjects.HandleContext<SimpleCommand>(resultType);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCastException>(() => invoker.InvokeHandleAsync(handler, context, resultType));
        }
    }
}