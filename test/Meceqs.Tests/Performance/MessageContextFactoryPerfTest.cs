using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Meceqs.Pipeline;
using Xunit;

namespace Meceqs.Tests.Performance
{
    [SuppressMessage("xUnit", "xUnit1000", Justification = "Make class public to actually run it.")]
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
            var messageType = typeof(SimpleMessage);

            // Activator.CreateInstance

            var typeCache = new ConcurrentDictionary<Type, Type>();

            RunTimed("Activator.CreateInstance", loopCount, () =>
            {
                var typedMessageContextType = typeCache.GetOrAdd(messageType, x =>
                {
                    return typeof(MessageContext<>).MakeGenericType(x);
                });

                MessageContext result = (MessageContext)Activator.CreateInstance(typedMessageContextType, envelope);
            });

            // ConstructorInfo.Invoke

            var constructorCache = new ConcurrentDictionary<Type, ConstructorInfo>();

            RunTimed("ConstructorInfo.Invoke", loopCount, () =>
            {
                var ctor = constructorCache.GetOrAdd(messageType, x =>
                {
                    var typedMessageContextType = typeof(MessageContext<>).MakeGenericType(x);
                    var constructor = typedMessageContextType.GetTypeInfo().DeclaredConstructors.First();

                    return constructor;
                });

                MessageContext result = (MessageContext)ctor.Invoke(new object[] { envelope });
            });

            // Compiled expression

            var delegateCache = new ConcurrentDictionary<Type, Func<Envelope, MessageContext>>();

            RunTimed("Expression.Lambda", loopCount, () =>
            {
                var ctorDelegate = delegateCache.GetOrAdd(messageType, x =>
                {
                    Type typedEnvelopeType = typeof(Envelope<>).MakeGenericType(x);
                    Type typedMessageContextType = typeof(MessageContext<>).MakeGenericType(x);

                    ConstructorInfo constructor = typedMessageContextType.GetTypeInfo().DeclaredConstructors.First();

                    var envelopeParam = Expression.Parameter(typeof(Envelope), "envelope");
                    var castedEnvelopeParam = Expression.Convert(envelopeParam, typedEnvelopeType);

                    var compiledDelegate = Expression.Lambda<Func<Envelope, MessageContext>>(
                        Expression.New(constructor, castedEnvelopeParam),
                        envelopeParam
                    ).Compile();

                    return compiledDelegate;
                });

                MessageContext result = ctorDelegate(envelope);
            });
        }
    }
}