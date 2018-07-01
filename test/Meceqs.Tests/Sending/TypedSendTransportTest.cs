// TODO !!!
// using System.Threading.Tasks;
// using Meceqs.Sending;
// using Meceqs.Sending.TypedSend;
// using Microsoft.Extensions.DependencyInjection;
// using NSubstitute;

// namespace Meceqs.Tests.Sending
// {
//     public class TypedSendTransportTest
//     {
//         private ISendTransport GetTransport(IServiceCollection services)
//         {
//             var serviceProvider = services.BuildServiceProvider();
//             var senderFactory = new DefaultSenderFactory(serviceProvider);

//             return new TypedSendTransport(senderFactory, new DefaultSenderFactoryInvoker(), new DefaultSenderInvoker());
//         }

//         private MessageContext<TMessage> GetSendContext<TMessage>()
//             where TMessage : class, IMessage, new()
//         {
//             var envelope = TestObjects.Envelope<TMessage>();
//             return new MessageContext<TMessage>(envelope);
//         }

//         //[Fact] // TODO @cweiss ! enable test
//         public async Task Calls_Matching_Handler()
//         {
//             // Arrange
//             var handler = Substitute.For<ISender<SimpleMessage, string>>();
//             var services = new ServiceCollection().AddSingleton(handler);
//             var transport = GetTransport(services);

//             var sendContext = GetSendContext<SimpleMessage>();

//             // Act
//             string response = await transport.SendAsync<string>(sendContext);

//             // Assert
//             await handler.Received(1).SendAsync(sendContext);
//         }
//     }
// }