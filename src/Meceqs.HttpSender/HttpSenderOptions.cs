using System;
using System.Collections.Generic;
using Meceqs.Transport;

namespace Meceqs.HttpSender
{
    public class HttpSenderOptions : TransportSenderOptions
    {
        /// <summary>
        /// Configures the mapping between message types and endpoint URIs for messages that
        /// are added via one of the <see cref="AddMessage"/> methods.
        /// </summary>
        public IEndpointMessageConvention MessageConvention { get; set; } = new DefaultEndpointMessageConvention();

        /// <summary>
        /// A dictionary containing message type to endpoint URI mappings that are supported with this sender.
        /// </summary>
        public Dictionary<Type, string> Messages { get; private set; } = new Dictionary<Type, string>();
    }
}