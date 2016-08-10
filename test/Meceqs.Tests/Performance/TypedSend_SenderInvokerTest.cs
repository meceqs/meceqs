using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Meceqs.Pipeline;
using Meceqs.Sending.TypedSend;
using Meceqs.Tests;
using Xunit;

namespace Meceqs.Test.Performance
{
    // Make class public to actually run it.
    public class TypedSend_SenderInvokerTest
    {
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

        private HandleContext<TMessage> GetSendContext<TMessage, TResult>()
            where TMessage : class, IMessage, new()
        {
            var envelope = TestObjects.Envelope<TMessage>();

            var filterContext = new FilterContext<TMessage>(envelope);
            filterContext.ExpectedResultType = typeof(TResult);

            var sendContext = new HandleContext<TMessage>(filterContext);

            return sendContext;
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
        public async Task SendAsync_MethodInfoInvoke_vs_CompiledExpression()
        {
            const int loopCount = 100000;

            var sender = new SimpleMessageStringSender("result");
            var context = GetSendContext<SimpleMessage, string>();

            Type messageType = typeof(SimpleMessage);
            Type resultType = typeof(string);

            // resolve types (necessary for all tests)

            // resolve generic types on ISender
            Type typedSenderType = typeof(IHandles<,>).MakeGenericType(messageType, resultType);

            // the MethodInfo must be created from a type which has resolved generic types,
            // otherwise the method has unresolved generic types.
            MethodInfo typedSendMethod = typedSenderType.GetMethod("SendAsync");

            // resolve generic types on parameters of SendAsync method
            Type typedSendContextType = typeof(HandleContext<>).MakeGenericType(messageType);


            // MethodInfo.Invoke
            // .................

            await RunTimed("Invoke SendAsync: MethodInfo.Invoke", loopCount, async () =>
            {
                Task resultTask = (Task) typedSendMethod.Invoke(sender, new [] { context });
                await resultTask;
            });

            // Compiled Expression
            // ...................

            
            // Create expression

            // Declaration of the object on which the method should be called
            var instance = Expression.Parameter(typeof(object), "instance");

            // Declaration of the parameters that should be passed to the call
            var contextArg = Expression.Parameter(typeof(object), "messageContext");

            // // Declaration of the actual method call
            var methodCall = Expression.Call(
                Expression.Convert(instance, typedSenderType), 
                typedSendMethod, 
                Expression.Convert(contextArg, typedSendContextType));

            // Compiles declaration into actual delegate
            var typedDelegate = Expression.Lambda<Func<object, object, Task>>(
                Expression.Convert(methodCall, typeof(Task)), instance, contextArg
            ).Compile();

            await RunTimed("Invoke SendAsync: Expression.Compile", loopCount, async () =>
            {
                Task resultTask = typedDelegate(sender, context);
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

            RunTimed("Get Task.Result: PropertyInfo.GetValue", loopCount, () =>
            {
                string result = (string) resultPropertyInfo.GetValue(task);
            });

            // Compiled expression

            var instance = Expression.Parameter(typeof(object), "instance");
            var getMethod = resultPropertyInfo.GetGetMethod();
            var methodCall = Expression.Call(Expression.Convert(instance, taskResultType), getMethod);
            var typedDelegate = Expression.Lambda<Func<object, object>>(methodCall, instance).Compile();

            RunTimed("Get Task.Result: Expression.Compile", loopCount, () =>
            {
                string result = (string) typedDelegate(task);
            });
        }
    }
}