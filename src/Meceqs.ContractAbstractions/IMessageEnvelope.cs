using System;

namespace Meceqs
{
    public interface IMessageEnvelope
    {
        MessageHeaders Headers { get; set; }

        Guid MessageId { get; set; }

        string MessageName { get; set; }

        string MessageType { get; set; }

        Guid CorrelationId { get; set; }
    }
}