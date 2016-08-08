using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Xunit;

namespace Meceqs.Tests.Performance
{
    public class MessageContextFactoryPerfTest
    {
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
        public void Create_Activator_vs_ConstructorInfoInvoke_vs_CompiledExpression()
        {
            const int loopCount = 100000;

            var envelope = TestObjects.Envelope<SimpleMessage>();
            var contextData = new MessageContextData();
            var cancellation = CancellationToken.None;

            var messageType = typeof(SimpleMessage);

            var typedMessageContext = typeof(MessageContext<>).MakeGenericType(messageType);

            // Activator.CreateInstance

            RunTimed("CreateContext: Activator.CreateInstance", loopCount, () =>
            {
                MessageContext result = (MessageContext)Activator.CreateInstance(typedMessageContext, envelope, contextData, cancellation);
            });

            // ConstructorInfo.Invoke

            ConstructorInfo constructor = typedMessageContext.GetTypeInfo().DeclaredConstructors.First();
            RunTimed("CreateContext: ConstructorInfo.Invoke", loopCount, () =>
            {
                MessageContext result = (MessageContext)constructor.Invoke(new object[] { envelope, contextData, cancellation });
            });

            // Compiled expression

            Type typedEnvelopeType = typeof(Envelope<>).MakeGenericType(messageType);
            var envelopeParam = Expression.Parameter(typeof(Envelope), "envelope");
            var castedEnvelopeParam = Expression.Convert(envelopeParam, typedEnvelopeType);
            var contextDataParam = Expression.Parameter(typeof(MessageContextData), "contextData");
            var cancellationParam = Expression.Parameter(typeof(CancellationToken), "cancellation");
            
            var ctorDelegate = Expression.Lambda<Func<Envelope, MessageContextData, CancellationToken, MessageContext>>(
                Expression.New(constructor, castedEnvelopeParam, contextDataParam, cancellationParam),
                envelopeParam, contextDataParam, cancellationParam
            ).Compile();
            
            RunTimed("CreateContext: Expression.Lambda", loopCount, () =>
            {
                MessageContext result = ctorDelegate(envelope, contextData, cancellation);
            });
        }
    }
}