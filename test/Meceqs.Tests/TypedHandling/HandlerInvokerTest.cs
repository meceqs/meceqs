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

        private HandleContext GetHandleContext<TMessage>(Type responseType, IHandles handler)
            where TMessage : class, new()
        {
            var messageContext = TestObjects.MessageContext<SimpleMessage>(responseType);

            var handleMethodResolver = new DefaultHandleMethodResolver();
            MethodInfo handleMethod = handleMethodResolver.GetHandleMethod(handler.GetType(), typeof(TMessage), responseType);

            return new HandleContext(messageContext, handler, handleMethod);
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
            context.MessageContext.Response.ShouldBe(1);
        }

        [Fact]
        public async Task Succeeds_for_SimpleMessage_without_response()
        {
            // Arrange
            var invoker = GetInvoker();
            bool handlerCalled = false;
            var handler = new SimpleMessageNoResponseHandler(() => handlerCalled = true);
            var context = GetHandleContext<SimpleMessage>(typeof(void), handler);

            // Act
            await invoker.InvokeHandleAsync(context);

            // Assert
            handlerCalled.ShouldBeTrue();
            context.MessageContext.Response.ShouldBeNull();
        }

        [Fact]
        public async Task Succeeds_for_SimpleMessage_and_SimpleResponse()
        {
            // Arrange
            var invoker = GetInvoker();
            var expectedResponse = new SimpleResponse();
            var handler = new SimpleMessageSimpleResponseHandler(expectedResponse);
            var context = GetHandleContext<SimpleMessage>(typeof(SimpleResponse), handler);

            // Act
            await invoker.InvokeHandleAsync(context);

            // Assert
            context.MessageContext.Response.ShouldBe(expectedResponse);
        }
    }
}
