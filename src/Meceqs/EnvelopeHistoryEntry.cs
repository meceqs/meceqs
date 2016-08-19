using System;

namespace Meceqs
{
    public class EnvelopeHistoryEntry
    {
        public string Pipeline { get; set; }
        public string Host { get; set; }
        public string Endpoint { get; set; }
        public DateTimeOffset CreatedOnUtc { get; set; }
        public EnvelopeProperties Properties { get; set; } = new EnvelopeProperties();
    }
}