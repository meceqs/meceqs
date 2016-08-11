using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Meceqs.Tests.Performance
{
    // Make class public to actually run it.
    internal class HandleUntypedEnvelopeTest
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

            Console.WriteLine($"{GetType().Name}/{message}: {sw.ElapsedMilliseconds} ms");
        }

        [Fact]
        public async Task Direct_vs_Dynamic_vs_MethodInfoInvoke()
        {
            const int loopCount = 100000;

            IEnvelopeHandler mediator = new DummyMediator();
            var envelope = TestObjects.Envelope<SimpleCommand>();

            var messageType = envelope.Message.GetType();
            var resultType = typeof(VoidType);

            MethodInfo handleMethod = typeof(IEnvelopeHandler).GetMethod(nameof(IEnvelopeHandler.HandleAsync));

            var cacheKey = new Tuple<Type, Type>(messageType, resultType);

            // Direct

            await RunTimed("Direct Typed", loopCount, async () =>
            {
                await mediator.HandleAsync(envelope, CancellationToken.None);
            });

            // dynamic

            await RunTimed("Dynamic", loopCount, async () =>
            {
                var dynamicEnvelope = (dynamic)envelope;
                await EnvelopeHandlerExtensions.HandleAsync(mediator, dynamicEnvelope, CancellationToken.None);
            });

            // MethodInfo.Invoke

            var methodInfoCache = new ConcurrentDictionary<Tuple<Type, Type>, MethodInfo>();

            await RunTimed("MethodInfo.Invoke", loopCount, async () =>
            {
                var method = methodInfoCache.GetOrAdd(cacheKey, x =>
                {
                    MethodInfo typedHandleMethod = handleMethod.MakeGenericMethod(x.Item1, x.Item2);
                    return typedHandleMethod;
                });

                await (Task)method.Invoke(mediator, new object[] { envelope, CancellationToken.None });
            });

            // Compiled Expression

            var delegateCache = new ConcurrentDictionary<Tuple<Type, Type>, Func<IEnvelopeHandler, Envelope, CancellationToken, Task>>();

            await RunTimed("Expression.Lambda", loopCount, async () =>
            {
                var del = delegateCache.GetOrAdd(cacheKey, x =>
                {
                    var envelopeType = typeof(Envelope<>).MakeGenericType(x.Item1);

                    var instance = Expression.Parameter(typeof(IEnvelopeHandler), "instance");
                    var envelopeParam = Expression.Parameter(typeof(Envelope), "envelope");
                    var castedEnvelopeParam = Expression.Convert(envelopeParam, envelopeType);
                    var cancellationParam = Expression.Parameter(typeof(CancellationToken), "cancellation");

                    MethodInfo typedHandleMethod = handleMethod.MakeGenericMethod(x.Item1, x.Item2);

                    var compiledCall = Expression.Lambda<Func<IEnvelopeHandler, Envelope, CancellationToken, Task>>(
                        Expression.Call(instance, typedHandleMethod, castedEnvelopeParam, cancellationParam),
                        instance, envelopeParam, cancellationParam
                    ).Compile();

                    return compiledCall;
                });

                await del(mediator, envelope, CancellationToken.None);
            });
        }
    }
}