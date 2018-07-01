using System;
using System.Collections.Generic;
using Meceqs.Transport;

namespace Meceqs.HttpSender
{
    public class HttpSenderOptions : SendTransportOptions
    {
        /// <summary>
        /// Configures the mapping between message types and endpoint URIs.
        /// </summary>
        public IEndpointMessageConvention MessageConvention { get; set; } = new DefaultEndpointMessageConvention();

        /// <summary>
        /// A dictionary containing message type to endpoint URI mappings that are supported with this sender.
        /// </summary>
        public Dictionary<Type, string> Messages { get; private set; } = new Dictionary<Type, string>();
    }
}