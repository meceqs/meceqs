using System;

namespace Meceqs.HttpSender
{
    /// <summary>
    /// Represents a mapping between a message type and its relative target URI within the endpoint.
    /// </summary>
    public class EndpointMessage
    {
        public Type MessageType { get; set; }

        public string RelativePath { get; set; }
    }
}