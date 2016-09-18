using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace Meceqs.HttpSender
{
    public class EndpointOptions
    {
        public IEndpointMessageConvention MessageConvention { get; set; } = new DefaultEndpointMessageConvention();

        public string BaseAddress { get; set; }

        public List<Type> Handlers { get; private set; } = new List<Type>();

        public List<EndpointMessage> Messages { get; private set; } = new List<EndpointMessage>();

        public void AddDelegatingHandler<TDelegatingHandler>()
            where TDelegatingHandler : DelegatingHandler
        {
            Handlers.Add(typeof(TDelegatingHandler));
        }

        public void AddDelegatingHandler(Type delegatingHandler)
        {
            Check.NotNull(delegatingHandler, nameof(delegatingHandler));

            if (!typeof(DelegatingHandler).IsAssignableFrom(delegatingHandler))
            {
                throw new ArgumentException($"'{delegatingHandler}' does not inherit from '{nameof(DelegatingHandler)}'");
            }

            Handlers.Add(delegatingHandler);
        }

        public void AddMessagesFromAssembly<TMessage>(Predicate<Type> filter)
        {
            AddMessagesFromAssembly(typeof(TMessage).GetTypeInfo().Assembly, filter);
        }

        public void AddMessagesFromAssembly(Assembly assembly, Predicate<Type> filter)
        {
            Check.NotNull(assembly, nameof(assembly));
            Check.NotNull(filter, nameof(filter));

            var messages = from type in assembly.GetTypes()
                           where type.GetTypeInfo().IsClass && !type.GetTypeInfo().IsAbstract
                           where filter(type)
                           select type;

            foreach (var message in messages)
            {
                AddMessage(message);
            }
        }

        public void AddMessage<TMessage>()
        {
            AddMessage(typeof(TMessage));
        }

        public void AddMessage(Type messageType)
        {
            Check.NotNull(messageType, nameof(messageType));

            var endpointMessage = MessageConvention.GetEndpointMessage(messageType);

            Messages.Add(endpointMessage);
        }
    }
}