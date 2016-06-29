using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.Handling;
using Xunit;

namespace Meceqs.Tests
{
    // Make class public to actually run it.

    internal class ReflectionTest
    {
        public class DummyMediator : IEnvelopeHandler
        {
            public async Task<TResult> HandleAsync<TMessage, TResult>(Envelope<TMessage> envelope, CancellationToken cancellation)
                where TMessage : IMessage
            {
                return await Task.FromResult<TResult>(default(TResult));
            }
        }

        private async Task RunTimed(string message, int loopCount, Func<Task> action)
        {
            Stopwatch sw = Stopwatch.StartNew();

            for (int i = 0; i < loopCount; i++)
            {
                await action();
            }

            sw.Stop();

            Console.WriteLine($"{message}: {sw.ElapsedMilliseconds} ms");
        }

        [Fact]
        public async Task Direct_vs_Dynamic_vs_MethodInfoInvoke()
        {
            const int loopCount = 100000;

            IEnvelopeHandler mediator = new DummyMediator();
            var envelope = TestObjects.Envelope<SimpleCommand>();
            
            // Direct
            
            await RunTimed("Direct", loopCount, async () => 
            {
                await mediator.HandleAsync(envelope, CancellationToken.None);
            });

            // dynamic

            await RunTimed("Dynamic", loopCount, async () => 
            {
                var dynamicEnvelope = (dynamic) envelope;
                await EnvelopeHandlerExtensions.HandleAsync(mediator, dynamicEnvelope, CancellationToken.None);
            });

            // MethodInfo.Invoke
            
            MethodInfo handleMethod = typeof(IEnvelopeHandler).GetMethod(nameof(IEnvelopeHandler.HandleAsync));
            await RunTimed("MethodInfo.Invoke", loopCount, async () => 
            {
                MethodInfo genericHandleMethod = handleMethod.MakeGenericMethod(typeof(SimpleCommand), typeof(VoidType));
                await (Task) genericHandleMethod.Invoke(mediator, new object[] { envelope, CancellationToken.None });
            });
        }
    }
}