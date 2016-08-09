using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Xunit;

namespace Meceqs.Tests.Performance
{
    // Make class public to actually run it.
    internal class MessageContextFactoryPerfTest
    {
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
        public void Create_Activator_vs_ConstructorInfoInvoke_vs_CompiledExpression()
        {
            const int loopCount = 100000;

            var envelope = TestObjects.Envelope<SimpleMessage>();
            var contextData = new MessageContextData();
            var cancellation = CancellationToken.None;

            var messageType = typeof(SimpleMessage);

            var typedMessageContext = typeof(MessageContext<>).MakeGenericType(messageType);

            // Activator.CreateInstance

            RunTimed("Activator.CreateInstance", loopCount, () =>
            {
                MessageContext result = (MessageContext)Activator.CreateInstance(typedMessageContext, envelope, contextData, cancellation);
            });

            // ConstructorInfo.Invoke

            var constructorCache = new ConcurrentDictionary<Type, ConstructorInfo>();

            RunTimed("ConstructorInfo.Invoke", loopCount, () =>
            {
                var ctor = constructorCache.GetOrAdd(messageType, x =>
                {

                    var typedMessageContext1 = typeof(MessageContext<>).MakeGenericType(x);
                    var constructor = typedMessageContext1.GetTypeInfo().DeclaredConstructors.First();

                    return constructor;
                });

                MessageContext result = (MessageContext)ctor.Invoke(new object[] { envelope, contextData, cancellation });
            });

            // Compiled expression

            var delegateCache = new ConcurrentDictionary<Type, Func<Envelope, MessageContextData, CancellationToken, MessageContext>>();

            RunTimed("Expression.Lambda", loopCount, () =>
            {
                var del = delegateCache.GetOrAdd(messageType, x =>
                {
                    var typedMessageContext1 = typeof(MessageContext<>).MakeGenericType(x);
                    var constructor = typedMessageContext1.GetTypeInfo().DeclaredConstructors.First();

                    Type typedEnvelopeType = typeof(Envelope<>).MakeGenericType(x);
                    var envelopeParam = Expression.Parameter(typeof(Envelope), "envelope");
                    var castedEnvelopeParam = Expression.Convert(envelopeParam, typedEnvelopeType);
                    var contextDataParam = Expression.Parameter(typeof(MessageContextData), "contextData");
                    var cancellationParam = Expression.Parameter(typeof(CancellationToken), "cancellation");

                    var ctorDelegate = Expression.Lambda<Func<Envelope, MessageContextData, CancellationToken, MessageContext>>(
                        Expression.New(constructor, castedEnvelopeParam, contextDataParam, cancellationParam),
                        envelopeParam, contextDataParam, cancellationParam
                    ).Compile();

                    return ctorDelegate;
                });

                MessageContext result = del(envelope, contextData, cancellation);
            });
        }
    }
}