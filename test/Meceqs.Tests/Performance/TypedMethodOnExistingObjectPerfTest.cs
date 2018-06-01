using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit;

namespace Meceqs.Tests.Performance
{
    [SuppressMessage("xUnit", "xUnit1000", Justification = "Make class public to actually run it.")]
    internal class TypedMethodOnExistingObjectPerfTest
    {
        public class BrokeredMessage
        {
            public T GetBody<T>()
            {
                return default(T);
            }
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
        public void Direct_vs_Dynamic_vs_MethodInfoInvoke()
        {
            const int loopCount = 100000;

            BrokeredMessage message = new BrokeredMessage();
            var resultType = typeof(string);

            MethodInfo getBodyMethod = typeof(BrokeredMessage).GetMethod(nameof(BrokeredMessage.GetBody));

            // Direct

            RunTimed("Direct Typed", loopCount, () =>
            {
                string s = message.GetBody<string>();
            });

            // MethodInfo.Invoke

            var methodCache = new ConcurrentDictionary<Type, MethodInfo>();

            RunTimed("MethodInfo.Invoke", loopCount, () =>
            {
                var method = methodCache.GetOrAdd(resultType, x => {
                    MethodInfo typedHandleMethod = getBodyMethod.MakeGenericMethod(x);
                    return typedHandleMethod;
                });
                
                string s = (string)method.Invoke(message, new object[] { });
            });

            // MethodInfo Delegate

            var delegateCache = new ConcurrentDictionary<Type, Func<BrokeredMessage, object>>();

            RunTimed("MethodInfo Delegate", loopCount, () =>
            {
                var del = delegateCache.GetOrAdd(resultType, x =>
                {
                    MethodInfo typedHandleMethod = getBodyMethod.MakeGenericMethod(x);
                    Func<BrokeredMessage, object> compiledDel = (Func<BrokeredMessage, object>) typedHandleMethod.CreateDelegate(typeof(Func<BrokeredMessage, object>), null);
                    return compiledDel;
                });

                string s = (string) del(message);
            });
        }
    }
}