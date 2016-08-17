using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Meceqs.Filters.TypedHandling;
using Meceqs.Filters.TypedHandling.Internal;
using Xunit;

namespace Meceqs.Tests.Filters.TypedHandling
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
            var handlerType = typeof(SimpleMessageNoResultHandler);
            var messageType = typeof(SimpleMessage);
            Type resultType = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => resolver.GetHandleMethod(null, messageType, resultType));
            Assert.Throws<ArgumentNullException>(() => resolver.GetHandleMethod(handlerType, null, resultType));
        }

        [Fact]
        public void Succeeds_for_SimpleMessage_no_result()
        {
            // Arrange
            var resolver = GetResolver();
            var handlerType = typeof(SimpleMessageNoResultHandler);
            var messageType = typeof(SimpleMessage);
            Type resultType = null;

            // Act
            var method = resolver.GetHandleMethod(handlerType, messageType, resultType);

            // Assert
            AssertMethod(method, messageType, resultType);
        }

        [Fact]
        public void Succeeds_for_SimpleMessage_int()
        {
            // Arrange
            var resolver = GetResolver();
            var handlerType = typeof(SimpleMessageIntHandler);
            var messageType = typeof(SimpleMessage);
            Type resultType = typeof(int);

            // Act
            var method = resolver.GetHandleMethod(handlerType, messageType, resultType);

            // Assert
            AssertMethod(method, messageType, resultType);
        }

        [Fact]
        public void Selects_SimpleMessageNoResult_if_handler_has_multiple_handlers()
        {
            // Arrange
            var resolver = GetResolver();
            var handlerType = typeof(MultipleMessagesHandler);
            var messageType = typeof(SimpleMessage);
            Type resultType = null;

            // Act
            var method = resolver.GetHandleMethod(handlerType, messageType, resultType);

            // Assert
            AssertMethod(method, messageType, resultType);
        }

        [Fact]
        public void Selects_SimpleCommandSimpleResult_if_handler_has_multiple_handlers()
        {
            // Arrange
            var resolver = GetResolver();
            var handlerType = typeof(MultipleMessagesHandler);
            var messageType = typeof(SimpleCommand);
            Type resultType = typeof(SimpleResult);

            // Act
            var method = resolver.GetHandleMethod(handlerType, messageType, resultType);

            // Assert
            AssertMethod(method, messageType, resultType);
        }

        [Fact]
        public void Selects_SimpleEventInt_if_handler_has_multiple_handlers()
        {
            // Arrange
            var resolver = GetResolver();
            var handlerType = typeof(MultipleMessagesHandler);
            var messageType = typeof(SimpleEvent);
            Type resultType = typeof(int);

            // Act
            var method = resolver.GetHandleMethod(handlerType, messageType, resultType);

            // Assert
            AssertMethod(method, messageType, resultType);
        }

        [Fact]
        public void Returns_null_if_handler_has_same_message_but_different_result()
        {
            // Arrange
            var resolver = GetResolver();
            var handlerType = typeof(SimpleMessageIntHandler);
            var messageType = typeof(SimpleMessage);
            Type resultType = typeof(string);

            // Act
            var method = resolver.GetHandleMethod(handlerType, messageType, resultType);

            // Assert
            Assert.Null(method);
        }

        [Fact]
        public void Returns_null_if_handler_has_same_message_but_no_result()
        {
            // Arrange
            var resolver = GetResolver();
            var handlerType = typeof(SimpleMessageNoResultHandler);
            var messageType = typeof(SimpleMessage);
            Type resultType = typeof(int);

            // Act
            var method = resolver.GetHandleMethod(handlerType, messageType, resultType);

            // Assert
            Assert.Null(method);
        }

        private void AssertMethod(MethodInfo method, Type messageType, Type resultType)
        {
            var handleContextType = typeof(HandleContext<>).MakeGenericType(messageType);

            var resultTaskType = resultType != null
                ? typeof(Task<>).MakeGenericType(resultType)
                : typeof(Task);

            Assert.NotNull(method);
            Assert.Equal(handleContextType, method.GetParameters().First().ParameterType);
            Assert.Equal(resultTaskType, method.ReturnType);
        }
    }
}