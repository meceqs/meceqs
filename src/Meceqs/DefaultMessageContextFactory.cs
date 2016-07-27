using System;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Meceqs
{
    public class DefaultMessageContextFactory : IMessageContextFactory
    {
        public MessageContext Create(Envelope envelope, MessageContextData contextData, CancellationToken cancellation)
        {
            Check.NotNull(envelope, nameof(envelope));
            Check.NotNull(contextData, nameof(contextData));

            Type messageType = envelope.Message.GetType();
            
            // TODO @cweiss Caching !!!
            Type genericMessageContext = typeof(MessageContext<>);
            Type typedMessageContext = genericMessageContext.MakeGenericType(messageType);
            ConstructorInfo constructor = typedMessageContext.GetTypeInfo().DeclaredConstructors.First();
            object context = constructor.Invoke(new object[] { envelope, contextData, cancellation });

            return (MessageContext)context;
        }
    }
}