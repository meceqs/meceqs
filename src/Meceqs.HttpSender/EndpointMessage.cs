using System;

namespace Meceqs.HttpSender
{
    public class EndpointMessage
    {
        public Type MessageType { get; set; }

        public string RelativePath { get; set; }
    }
}