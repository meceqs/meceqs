using System;
using System.Reflection;
using System.Threading.Tasks;
using Meceqs.TypedHandling;
using Meceqs.TypedHandling.Internal;
using Shouldly;
using Xunit;

namespace Meceqs.Tests.Middleware.TypedHandling
{
    public class HandleMethodResolverTest
    {
        private IHandleMethodResolver GetResolver()
        {
            return new DefaultHandleMethodResolver();
        }

        [Fact]
        public void Throws_if_parameters_missing()
        {
            // Arrange
            var resolver = GetResolver();
            var handlerType = typeof(SimpleMessageNoResponseHandler);
            var messageType = typeof(SimpleMessage);
            Type responseType = typeof(void);

            // Act & Assert
            Should.Throw<ArgumentNullException>(() => resolver.GetHandleMethod(null, messageType, responseType));
            Should.Throw<ArgumentNullException>(() => resolver.GetHandleMethod(handlerType, null, responseType));
        }

        [Fact]
        public void Succeeds_for_SimpleMessage_no_response()
        {
            // Arrange
            var resolver = GetResolver();
            var handlerType = typeof(SimpleMessageNoResponseHandler);
            var messageType = typeof(SimpleMessage);
            Type responseType = typeof(void);

            // Act
            var method = resolver.GetHandleMethod(handlerType, messageType, responseType);

            // Assert
            AssertMethod(method, messageType, responseType);
        }

        [Fact]
        public void Succeeds_for_SimpleMessage_int()
        {
            // Arrange
            var resolver = GetResolver();
            var handlerType = typeof(SimpleMessageIntHandler);
            var messageType = typeof(SimpleMessage);
            Type responseType = typeof(int);

            // Act
            var method = resolver.GetHandleMethod(handlerType, messageType, responseType);

            // Assert
            AssertMethod(method, messageType, responseType);
        }

        [Fact]
        public void Selects_SimpleMessageNoResponse_if_handler_has_multiple_handlers()
        {
            // Arrange
            var resolver = GetResolver();
            var handlerType = typeof(MultipleMessagesHandler);
            var messageType = typeof(SimpleMessage);
            Type responseType = typeof(void);

            // Act
            var method = resolver.GetHandleMethod(handlerType, messageType, responseType);

            // Assert
            AssertMethod(method, messageType, responseType);
        }

        [Fact]
        public void Selects_SimpleCommandSimpleResponse_if_handler_has_multiple_handlers()
        {
            // Arrange
            var resolver = GetResolver();
            var handlerType = typeof(MultipleMessagesHandler);
            var messageType = typeof(SimpleCommand);
            Type responseType = typeof(SimpleResponse);

            // Act
            var method = resolver.GetHandleMethod(handlerType, messageType, responseType);

            // Assert
            AssertMethod(method, messageType, responseType);
        }

        [Fact]
        public void Selects_SimpleEventInt_if_handler_has_multiple_handlers()
        {
            // Arrange
            var resolver = GetResolver();
            var handlerType = typeof(MultipleMessagesHandler);
            var messageType = typeof(SimpleEvent);
            Type responseType = typeof(int);

            // Act
            var method = resolver.GetHandleMethod(handlerType, messageType, responseType);

            // Assert
            AssertMethod(method, messageType, responseType);
        }

        [Fact]
        public void Returns_null_if_handler_has_same_message_but_different_response()
        {
            // Arrange
            var resolver = GetResolver();
            var handlerType = typeof(SimpleMessageIntHandler);
            var messageType = typeof(SimpleMessage);
            Type responseType = typeof(string);

            // Act
            var method = resolver.GetHandleMethod(handlerType, messageType, responseType);

            // Assert
            method.ShouldBeNull();
        }

        [Fact]
        public void Returns_null_if_handler_has_same_message_but_no_response()
        {
            // Arrange
            var resolver = GetResolver();
            var handlerType = typeof(SimpleMessageNoResponseHandler);
            var messageType = typeof(SimpleMessage);
            Type responseType = typeof(int);

            // Act
            var method = resolver.GetHandleMethod(handlerType, messageType, responseType);

            // Assert
            method.ShouldBeNull();
        }

        private void AssertMethod(MethodInfo method, Type messageType, Type responseType)
        {
            var responseTaskType = responseType != typeof(void)
                ? typeof(Task<>).MakeGenericType(responseType)
                : typeof(Task);

            method.ShouldNotBeNull();
            method.GetParameters()[0].ParameterType.ShouldBe(messageType);
            method.GetParameters()[1].ParameterType.ShouldBe(typeof(HandleContext));
            method.ReturnType.ShouldBe(responseTaskType);
        }
    }
}
