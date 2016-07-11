using System;
using System.Diagnostics;
using System.Reflection;
using Xunit;

namespace Meceqs.Tests.Performance
{
    internal class TypedMethodOnExistingObjectTest
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

            Console.WriteLine($"{message}: {sw.ElapsedMilliseconds} ms");
        }

        [Fact]
        public void Direct_vs_Dynamic_vs_MethodInfoInvoke()
        {
            const int loopCount = 100000;

            BrokeredMessage message = new BrokeredMessage();

            // Direct

            RunTimed("Direct Typed", loopCount, () =>
            {
                string s = message.GetBody<string>();
            });

            // MethodInfo.Invoke

            MethodInfo getBodyMethod1 = typeof(BrokeredMessage).GetMethod(nameof(BrokeredMessage.GetBody));
            RunTimed("MethodInfo.Invoke", loopCount, () =>
            {

                MethodInfo genericHandleMethod = getBodyMethod1.MakeGenericMethod(typeof(string));
                string s = (string)genericHandleMethod.Invoke(message, new object[] { });
            });

            // MethodInfo Delegate

            MethodInfo getBodyMethod2 = typeof(BrokeredMessage).GetMethod(nameof(BrokeredMessage.GetBody));
            MethodInfo getBodyMethodString = getBodyMethod2.MakeGenericMethod(typeof(string));
            Func<BrokeredMessage, object> delString = (Func<BrokeredMessage, object>) Delegate.CreateDelegate(typeof(Func<BrokeredMessage, object>), null, getBodyMethodString, true);

            RunTimed("MethodInfo Delegate", loopCount, () =>
            {
                string s = (string) delString(message);
            });
        }
    }
}