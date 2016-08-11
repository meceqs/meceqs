using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Meceqs.Pipeline;
using Xunit;

namespace Meceqs.Tests.Performance
{
    // Make class public to actually run it.
    internal class FilterContextFactoryPerfTest
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
                var typedFilterContextType = typeCache.GetOrAdd(messageType, x =>
                {
                    return typeof(FilterContext<>).MakeGenericType(x);
                });

                FilterContext result = (FilterContext)Activator.CreateInstance(typedFilterContextType, envelope);
            });

            // ConstructorInfo.Invoke

            var constructorCache = new ConcurrentDictionary<Type, ConstructorInfo>();

            RunTimed("ConstructorInfo.Invoke", loopCount, () =>
            {
                var ctor = constructorCache.GetOrAdd(messageType, x =>
                {
                    var typedFilterContextType = typeof(FilterContext<>).MakeGenericType(x);
                    var constructor = typedFilterContextType.GetTypeInfo().DeclaredConstructors.First();

                    return constructor;
                });

                FilterContext result = (FilterContext)ctor.Invoke(new object[] { envelope });
            });

            // Compiled expression

            var delegateCache = new ConcurrentDictionary<Type, Func<Envelope, FilterContext>>();

            RunTimed("Expression.Lambda", loopCount, () =>
            {
                var ctorDelegate = delegateCache.GetOrAdd(messageType, x =>
                {
                    Type typedEnvelopeType = typeof(Envelope<>).MakeGenericType(x);
                    Type typedFilterContextType = typeof(FilterContext<>).MakeGenericType(x);

                    ConstructorInfo constructor = typedFilterContextType.GetTypeInfo().DeclaredConstructors.First();

                    var envelopeParam = Expression.Parameter(typeof(Envelope), "envelope");
                    var castedEnvelopeParam = Expression.Convert(envelopeParam, typedEnvelopeType);

                    var compiledDelegate = Expression.Lambda<Func<Envelope, FilterContext>>(
                        Expression.New(constructor, castedEnvelopeParam),
                        envelopeParam
                    ).Compile();

                    return compiledDelegate;
                });

                FilterContext result = ctorDelegate(envelope);
            });
        }
    }
}