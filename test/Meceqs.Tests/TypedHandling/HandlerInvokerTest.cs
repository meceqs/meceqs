using System;
using System.Reflection;
using System.Threading.Tasks;
using Meceqs.TypedHandling;
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

        private HandleContext GetHandleContext<TMessage>(Type resultType, IHandles handler)
            where TMessage : class, new()
        {
            var context = TestObjects.HandleContext<SimpleMessage>(resultType);

            var handleMethodResolver = new DefaultHandleMethodResolver();
            MethodInfo handleMethod = handleMethodResolver.GetHandleMethod(handler.GetType(), typeof(TMessage), resultType);

            context.Initialize(handler, handleMethod);

            return context;
        }

        [Fact]
        public async Task Throws_if_parameters_are_missing()
        {
            // Arrange
            var invoker = GetInvoker();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => invoker.InvokeHandleAsync(null));
        }

        [Fact]
        public async Task Succeeds_for_SimpleMessage_and_int()
        {
            // Arrange
            var invoker = GetInvoker();
            var handler = new SimpleMessageIntHandler(1);
            var context = GetHandleContext<SimpleMessage>(typeof(int), handler);

            // Act
            await invoker.InvokeHandleAsync(context);

            // Assert
            context.MessageContext.Result.ShouldBe(1);
        }

        [Fact]
        public async Task Succeeds_for_SimpleMessage_without_result()
        {
            // Arrange
            var invoker = GetInvoker();
            bool handlerCalled = false;
            var handler = new SimpleMessageNoResultHandler(() => handlerCalled = true);
            var context = GetHandleContext<SimpleMessage>(typeof(void), handler);

            // Act
            await invoker.InvokeHandleAsync(context);

            // Assert
            handlerCalled.ShouldBeTrue();
            context.MessageContext.Result.ShouldBeNull();
        }

        [Fact]
        public async Task Succeeds_for_SimpleMessage_and_SimpleResult()
        {
            // Arrange
            var invoker = GetInvoker();
            var expectedResult = new SimpleResult();
            var handler = new SimpleMessageSimpleResultHandler(expectedResult);
            var context = GetHandleContext<SimpleMessage>(typeof(SimpleResult), handler);

            // Act
            await invoker.InvokeHandleAsync(context);

            // Assert
            context.MessageContext.Result.ShouldBe(expectedResult);
        }
    }
}