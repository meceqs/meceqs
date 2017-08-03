using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace Meceqs.HttpSender
{
    public class EndpointOptions
    {
        /// <summary>
        /// Configures the mapping between message types and endpoint URIs for messages that
        /// are added via one of the <see cref="AddMessage"/> methods.
        /// </summary>
        public IEndpointMessageConvention MessageConvention { get; set; } = new DefaultEndpointMessageConvention();

        /// <summary>
        /// The base URI of the endpoint. Message requests will add a relative path
        /// based on the configured <see cref="MessageConvention"/>.
        /// </summary>
        public string BaseAddress { get; set; }

        /// <summary>
        /// A list of <see cref="DelegatingHandler"/> that will be used to create the underlying <see cref="HttpClient"/>.
        /// The handlers will be chained together. The first handler in the list will be the outermost handler.
        /// </summary>
        public List<Type> DelegatingHandlers { get; private set; } = new List<Type>();

        /// <summary>
        /// A list of message type to endpoint URI mappings that are supported with this sender.
        /// </summary>
        public List<EndpointMessage> Messages { get; private set; } = new List<EndpointMessage>();

        public void AddDelegatingHandler<TDelegatingHandler>()
            where TDelegatingHandler : DelegatingHandler
        {
            DelegatingHandlers.Add(typeof(TDelegatingHandler));
        }

        public void AddDelegatingHandler(Type delegatingHandler)
        {
            Guard.NotNull(delegatingHandler, nameof(delegatingHandler));

            if (!typeof(DelegatingHandler).IsAssignableFrom(delegatingHandler))
            {
                throw new ArgumentException($"'{delegatingHandler}' does not inherit from '{nameof(DelegatingHandler)}'");
            }

            DelegatingHandlers.Add(delegatingHandler);
        }

        public void AddMessagesFromAssembly<TMessage>(Predicate<Type> filter)
        {
            AddMessagesFromAssembly(typeof(TMessage).GetTypeInfo().Assembly, filter);
        }

        public void AddMessagesFromAssembly(Assembly assembly, Predicate<Type> filter)
        {
            Guard.NotNull(assembly, nameof(assembly));
            Guard.NotNull(filter, nameof(filter));

            var messages = from type in assembly.GetTypes()
                           where type.GetTypeInfo().IsClass && !type.GetTypeInfo().IsAbstract
                           where filter(type)
                           select type;

            foreach (var message in messages)
            {
                AddMessage(message);
            }
        }

        /// <summary>
        /// Add the given message type to this sender. The relative endpoint path will be resolved
        /// by using the configured <see cref="MessageConvention"/>.
        /// </summary>
        public void AddMessage<TMessage>()
        {
            AddMessage(typeof(TMessage));
        }

        /// <summary>
        /// Add the given message type to this sender. The relative endpoint path will be resolved
        /// by using the configured <see cref="MessageConvention"/>.
        /// </summary>
        public void AddMessage(Type messageType)
        {
            Guard.NotNull(messageType, nameof(messageType));

            var endpointMessage = MessageConvention.GetEndpointMessage(messageType);

            Messages.Add(endpointMessage);
        }
    }
}