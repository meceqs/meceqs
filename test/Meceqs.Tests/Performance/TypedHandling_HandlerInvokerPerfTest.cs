using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Meceqs.Pipeline;
using Meceqs.Tests;
using Meceqs.TypedHandling;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Meceqs.Test.Performance
{
    [SuppressMessage("xUnit", "xUnit1000", Justification = "Make class public to actually run it.")]
    public class TypedHandling_HandlerInvokerPerfTest
    {
        private class SimpleMessageStringHandler : IHandles<SimpleMessage, string>
        {
            private readonly string _result;

            public SimpleMessageStringHandler(string result)
            {
                _result = result;
            }

            public Task<string> HandleAsync(SimpleMessage msg, HandleContext context)
            {
                return Task.FromResult(_result);
            }
        }

        private MessageContext GetMessageContext<TMessage, TResult>()
            where TMessage : class, new()
        {
            var envelope = TestObjects.Envelope<TMessage>();
            var serviceProvider = new ServiceCollection().BuildServiceProvider();

            return new MessageContext(envelope, "pipeline", serviceProvider, typeof(TResult));
        }

        private async Task RunTimed(string message, int loopCount, Func<Task> action)
        {
            Stopwatch sw = Stopwatch.StartNew();

            for (int i = 0; i < loopCount; i++)
            {
                await action();
            }

            sw.Stop();

            Console.WriteLine($"{GetType().Name}/{message}: {sw.ElapsedMilliseconds} ms");
        }

        private void RunTimed(string message, int loopCount, Action action)
        {
            Stopwatch sw = Stopwatch.StartNew();

            for (int i = 0; i < loopCount; i++)
            {
                action();
            }

            sw.Stop();

            Console.WriteLine($"{GetType().Name}/{message}: {sw.ElapsedMilliseconds} ms");
        }

        [Fact]
        public async Task HandleAsync_MethodInfoInvoke_vs_CompiledExpression()
        {
            const int loopCount = 100000;

            Type messageType = typeof(SimpleMessage);
            Type resultType = typeof(string);

            var messageContext = GetMessageContext<SimpleMessage, string>();
            var handler = new SimpleMessageStringHandler("result");

            // resolve types (necessary for all tests)

            // resolve generic types
            Type typedHandlerType = typeof(IHandles<,>).MakeGenericType(messageType, resultType);

            // the MethodInfo must be created from a type which has resolved generic types,
            // otherwise the method has unresolved generic types.
            MethodInfo typedHandleMethod = typedHandlerType.GetMethod("HandleAsync");

            var handleContext = new HandleContext(messageContext, handler, typedHandleMethod);


            // MethodInfo.Invoke
            // .................

            await RunTimed("HandleAsync: MethodInfo.Invoke", loopCount, async () =>
            {
                Task resultTask = (Task) typedHandleMethod.Invoke(handler, new [] { handleContext.Message, handleContext });
                await resultTask;
            });

            // Compiled Expression
            // ...................


            // Create expression

            // Declaration of the object on which the method should be called
            var instance = Expression.Parameter(typeof(object), "instance");

            // Declaration of the parameters that should be passed to the call
            var messageArg = Expression.Parameter(typeof(object), "message");
            var contextArg = Expression.Parameter(typeof(HandleContext), "handleContext");

            // // Declaration of the actual method call
            var methodCall = Expression.Call(
                Expression.Convert(instance, typedHandlerType),
                typedHandleMethod,
                Expression.Convert(messageArg, messageType),
                contextArg);

            // Compiles declaration into actual delegate
            var typedDelegate = Expression.Lambda<Func<object, object, HandleContext, Task>>(
                Expression.Convert(methodCall, typeof(Task)), instance, messageArg, contextArg
            ).Compile();

            await RunTimed("HandleAsync: Expression.Compile", loopCount, async () =>
            {
                Task resultTask = typedDelegate(handler, handleContext.Message, handleContext);
                await resultTask;
            });
        }

        [Fact]
        public void TaskResult_PropertyInfoInvoke_vs_CompiledExpression()
        {
            const int loopCount = 100000;

            Task task = Task.FromResult("test");

            Type taskResultType = typeof(Task<>).MakeGenericType(typeof(string));
            PropertyInfo resultPropertyInfo = taskResultType.GetProperty("Result");

            // PropertyInfo.GetValue

            RunTimed("Task.Result: PropertyInfo.GetValue", loopCount, () =>
            {
                string result = (string) resultPropertyInfo.GetValue(task);
            });

            // Compiled expression

            var instance = Expression.Parameter(typeof(object), "instance");
            var getMethod = resultPropertyInfo.GetGetMethod();
            var methodCall = Expression.Call(Expression.Convert(instance, taskResultType), getMethod);
            var typedDelegate = Expression.Lambda<Func<object, object>>(methodCall, instance).Compile();

            RunTimed("Task.Result: Expression.Compile", loopCount, () =>
            {
                string result = (string) typedDelegate(task);
            });
        }
    }
}