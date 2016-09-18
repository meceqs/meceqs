using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Meceqs.HttpSender
{
    public class EndpointOptions
    {
        public IEndpointMessageConvention MessageConvention { get; set; } = new DefaultEndpointMessageConvention();

        public string BaseAddress { get; set; }

        public List<EndpointMessage> Messages { get; set; } = new List<EndpointMessage>();

        public void AddFromAssembly<TMessage>(Predicate<Type> filter)
        {
            AddFromAssembly(typeof(TMessage).GetTypeInfo().Assembly, filter);
        }

        public void AddFromAssembly(Assembly assembly, Predicate<Type> filter)
        {
            Check.NotNull(assembly, nameof(assembly));
            Check.NotNull(filter, nameof(filter));

            var messages = from type in assembly.GetTypes()
                           where type.GetTypeInfo().IsClass && !type.GetTypeInfo().IsAbstract
                           where filter(type)
                           select type;

            foreach (var message in messages)
            {
                AddMessageType(message);
            }
        }

        public void AddMessageType<TMessage>()
        {
            AddMessageType(typeof(TMessage));
        }

        public void AddMessageType(Type messageType)
        {
            Check.NotNull(messageType, nameof(messageType));

            var endpointMessage = MessageConvention.GetEndpointMessage(messageType);

            Messages.Add(endpointMessage);
        }
    }
}