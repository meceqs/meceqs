using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Xunit;

namespace Meceqs.Tests.Performance
{
    // Make class public to actually run it.
    internal class LoadEnvelopeTypePerfTest
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
        public void Direct_Reflection_Cached()
        {
            const int loopCount = 100000;
            string messageType = typeof(SimpleMessage).FullName;

            // Direct

            RunTimed("Direct", loopCount, () =>
            {
                Type t = typeof(Envelope<SimpleMessage>);
            });

            // Reflection

            RunTimed("Reflection", loopCount, () =>
            {
                Type typeOfMessage = Type.GetType(messageType, throwOnError: true);
                Type envelopeType = typeof(Envelope<>).MakeGenericType(typeOfMessage);
            });

            // Cached Reflection

            ConcurrentDictionary<string, Type> cache = new ConcurrentDictionary<string, Type>();

            RunTimed("Cached", loopCount, () =>
            {
                Type t = cache.GetOrAdd(messageType, x =>
                {
                    Type typeOfMessage = Type.GetType(messageType, throwOnError: true);
                    return typeof(Envelope<>).MakeGenericType(typeOfMessage);
                });
            });
        }
    }
}