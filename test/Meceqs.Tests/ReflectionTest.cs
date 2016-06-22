// using System;
// using System.Diagnostics;
// using System.Reflection;
// using System.Threading;
// using System.Threading.Tasks;
// using Meceqs.Handling;
// using Xunit;

// namespace Meceqs.Tests
// {
//     public class ReflectionTest
//     {
//         public class MyCommand : ICommand
//         {
//             public string Property { get; set; }
//         }

//         public class DummyMediator : IMessageHandlingMediator
//         {
//             public async Task<TResult> HandleAsync<TMessage, TResult>(MessageEnvelope<TMessage> envelope, CancellationToken cancellation) where TMessage : IMessage
//             {
//                 return await Task.FromResult<TResult>(default(TResult));
//             }
//         }

//         private MessageEnvelope<TMessage> GetEnvelope<TMessage>() where TMessage : IMessage, new()
//         {
//             var msg = new TMessage();
//             return new MessageEnvelope<TMessage>(Guid.NewGuid(), msg);
//         }

//         private async Task RunTimed(string message, int loopCount, Func<Task> action)
//         {
//             Stopwatch sw = Stopwatch.StartNew();

//             for (int i = 0; i < loopCount; i++)
//             {
//                 await action();
//             }

//             sw.Stop();

//             Console.WriteLine($"{message}: {sw.ElapsedMilliseconds} ms");
//         }

//         [Fact]
//         public async Task Reflection()
//         {
//             const int loopCount = 100000;

//             IMessageHandlingMediator mediator = new DummyMediator();
//             //var envelope = GetEnvelope<MyCommand>();
            
//             // Direct
            
//             await RunTimed("Direct", loopCount, async () => 
//             {
//                 var envelope = GetEnvelope<MyCommand>();
//                 await mediator.HandleAsync(envelope, CancellationToken.None);
//             });

//             // dynamic

//             await RunTimed("Dynamic", loopCount, async () => 
//             {
//                 var dynamicEnvelope = (dynamic) GetEnvelope<MyCommand>();
//                 await MessageHandlingMediatorExtensions.HandleAsync(mediator, dynamicEnvelope, CancellationToken.None);
//             });

//             // MethodInfo.Invoke
            
//             MethodInfo handleMethod = typeof(IMessageHandlingMediator).GetMethod(nameof(IMessageHandlingMediator.HandleAsync));
//             await RunTimed("MethodInfo.Invoke", loopCount, async () => 
//             {
//                 var envelope = GetEnvelope<MyCommand>();
//                 MethodInfo genericHandleMethod = handleMethod.MakeGenericMethod(typeof(MyCommand), typeof(VoidType));
//                 await (Task) genericHandleMethod.Invoke(mediator, new object[] { envelope, CancellationToken.None });
//             });
            
//             //await mediator.Received(1).HandleAsync<MyCommand, VoidType>(envelope, CancellationToken.None);
//         }
//     }
// }